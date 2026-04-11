using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PJGoFast.Models;
using PJGoFast.Models.Enums;
using PJGoFast.Services.Interfaces;
using PJGoFast.ViewModels;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;

namespace PJGoFast.Controllers
{
    [Authorize(Roles = "KhachHang")]
    public class HomeController : Controller
    {
        private readonly IChuyenDiService _chuyenDiService;
        private readonly IKhachHangService _khachHangService;
        private readonly IHttpClientFactory _httpClientFactory;

        // ===== KEY API GOONG =====
        private const string GOONG_API_KEY = "H1eRRpu0DSAzfyP9x24rHLNX7OVocfHYoxVhCvGX";
        private const string GOONG_MAP_KEY = "DCZhv9n0uqtUA9sxXclNcEXFRlGdvUIY7LVngNWp";

        // ===== BẢNG GIÁ CƯỚC (VND/km) — CHỈNH SỬA TẠI ĐÂY NẾU CẦN =====
        private static readonly Dictionary<string, (decimal PerKm, decimal ToiThieu)> _bangGia = new()
        {
            { "XeMay",  (  8_000m,  15_000m) },
            { "Sedan",  ( 12_000m,  30_000m) },
            { "SUV",    ( 15_000m,  40_000m) },
            { "BayCho", ( 18_000m,  50_000m) },
            { "TaiNho", ( 22_000m,  60_000m) },
        };
        // ====================================================================

        public HomeController(
            IChuyenDiService chuyenDiService,
            IKhachHangService khachHangService,
            IHttpClientFactory httpClientFactory)
        {
            _chuyenDiService = chuyenDiService;
            _khachHangService = khachHangService;
            _httpClientFactory = httpClientFactory;
        }

        // ─── Index ────────────────────────────────────────────────────────────
        public IActionResult Index() => View();

        // ─── Tạo chuyến đi ───────────────────────────────────────────────────
        [HttpGet]
        public IActionResult taoChuyenDi()
        {
            var model = new ChuyenDiCreateVM { IdKH = User.FindFirstValue(ClaimTypes.Name) };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> taoChuyenDi(ChuyenDiCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Vui lòng điền đầy đủ thông tin và đảm bảo dữ liệu hợp lệ.";
                return View(model);
            }

            string result = _chuyenDiService.TaoChuyenDi(
                model.DiemDon, model.DiemDen,
                model.ThoiGianDon, model.LoaiXeYeuCau,
                model.GhiChu, model.IdKH);

            if (result != "0")
                return RedirectToAction(nameof(baoGia), new { IdChuyenDi = result });

            ViewBag.Error = "Đã xảy ra lỗi khi tạo chuyến đi. Vui lòng thử lại.";
            return View(model);
        }

        [HttpPost]
        public IActionResult taoChuyenDiWDiemDen(string diemDen)
        {
            var model = new ChuyenDiCreateVM
            {
                IdKH = User.FindFirstValue(ClaimTypes.Name),
                DiemDen = diemDen
            };
            return View("taoChuyenDi", model);
        }

        [HttpPost]
        public IActionResult taoChuyenDiWVehicle(LoaiXe loaiXe)
        {
            var model = new ChuyenDiCreateVM
            {
                IdKH = User.FindFirstValue(ClaimTypes.Name),
                LoaiXeYeuCau = loaiXe
            };
            return View("taoChuyenDi", model);
        }




        // ─── Báo giá (basic) ─────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> baoGia(string IdChuyenDi)
        {
            var vm = await BuildBaoGiaVMAsync(IdChuyenDi);
            if (vm == null) return NotFound();
            _chuyenDiService.CapNhatGiaTamTinh(IdChuyenDi, vm.GiaTamTinh);
            return View(vm);
        }

        // ─── Báo giá (map) ───────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> baoGiaMap(string IdChuyenDi)
        {
            var vm = await BuildBaoGiaVMAsync(IdChuyenDi);
            if (vm == null) return NotFound();
            vm.GoongMapKey = GOONG_MAP_KEY;
            vm.GoongApiKey = GOONG_API_KEY;
            return View(vm);
        }

        // ─── Xóa hẳn khỏi DB (từ baoGia / baoGiaMap) ────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XoaChuyenDi(string IdChuyenDi)
        {
            var idKH = User.FindFirstValue(ClaimTypes.Name);
            _chuyenDiService.XoaChuyenDi(IdChuyenDi, idKH);
            return RedirectToAction(nameof(Index));
        }

        // ─── Lịch sử ─────────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult History()
        {
            var idKH = User.FindFirstValue(ClaimTypes.Name);
            var list = _chuyenDiService.LayDanhSachChuyenDiCuaKH(idKH);
            return View(list);
        }

        // ─── Chi tiết chuyến đi ───────────────────────────────────────────────
        [HttpGet]
        public IActionResult ChiTietChuyenDi(string IdChuyenDi)
        {
            var idKH     = User.FindFirstValue(ClaimTypes.Name);
            var chuyenDi = _chuyenDiService.LayChuyenDiTheoId(IdChuyenDi);
            if (chuyenDi == null || chuyenDi.IdKH != idKH) return NotFound();
            return View(chuyenDi);
        }

        // ─── Hủy chuyến (chuyển trạng thái → HUY) ────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult HuyChuyenDi(string IdChuyenDi)
        {
            var idKH = User.FindFirstValue(ClaimTypes.Name);
            var ok   = _chuyenDiService.HuyChuyenDi(IdChuyenDi, idKH);

            TempData[ok ? "Success" : "Error"] = ok
                ? "Chuyến đi đã được hủy thành công."
                : "Không thể hủy chuyến đi này.";

            return RedirectToAction(nameof(History));
        }

        // ─── Tài khoản ───────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Account()
        {
            var idKH = User.FindFirstValue(ClaimTypes.Name);
            var kh   = _khachHangService.LayThongTinKhachHang(idKH);
            if (kh == null) return NotFound();
            return View(kh);
        }

        // ─── Helpers ─────────────────────────────────────────────────────────
        private async Task<BaoGiaVM?> BuildBaoGiaVMAsync(string idChuyenDi)
        {
            var chuyenDi = _chuyenDiService.LayChuyenDiTheoId(idChuyenDi);
            if (chuyenDi == null) return null;

            var client  = _httpClientFactory.CreateClient();
            var taskDon = GeocodeAsync(client, chuyenDi.DiemDon);
            var taskDen = GeocodeAsync(client, chuyenDi.DiemDen);
            await Task.WhenAll(taskDon, taskDen);

            var (donLat, donLng) = taskDon.Result;
            var (denLat, denLng) = taskDen.Result;

            double khoangCachKm = 0;
            int    thoiGianPhut = 0;

            if (donLat != 0 && denLat != 0)
            {
                var dmUrl = $"https://rsapi.goong.io/v2/distancematrix" +
                            $"?origins={donLat.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                            $",{donLng.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                            $"&destinations={denLat.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                            $",{denLng.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                            $"&vehicle=car&api_key={GOONG_API_KEY}";
                try
                {
                    var dmJson  = await client.GetStringAsync(dmUrl);
                    using var doc = JsonDocument.Parse(dmJson);
                    var el = doc.RootElement.GetProperty("rows")[0].GetProperty("elements")[0];
                    if (el.GetProperty("status").GetString() == "OK")
                    {
                        khoangCachKm = Math.Round(el.GetProperty("distance").GetProperty("value").GetInt32() / 1000.0, 1);
                        thoiGianPhut = (int)Math.Ceiling(el.GetProperty("duration").GetProperty("value").GetInt32() / 60.0);
                    }
                }
                catch { }
            }

            var (giaTamTinh, giaPerKm, apDungMin) = TinhGia(chuyenDi.LoaiXeYeuCau.ToString(), (decimal)khoangCachKm);

            return new BaoGiaVM
            {
                IdChuyenDi         = idChuyenDi,
                DiemDon            = chuyenDi.DiemDon,
                DiemDen            = chuyenDi.DiemDen,
                ThoiGianDon        = chuyenDi.ThoiGianDon,
                LoaiXeYeuCau       = chuyenDi.LoaiXeYeuCau,
                GhiChu             = chuyenDi.GhiChu,
                KhoangCachKm       = khoangCachKm,
                ThoiGianDuTinhPhut = thoiGianPhut,
                GiaTamTinh         = giaTamTinh,
                GiaPerKm           = giaPerKm,
                ApDungGiaToiThieu  = apDungMin,
                DonLat = donLat, DonLng = donLng,
                DenLat = denLat, DenLng = denLng,
            };
        }

        private async Task<(double lat, double lng)> GeocodeAsync(HttpClient client, string address)
        {
            try
            {
                var url  = $"https://rsapi.goong.io/v2/geocode?address={Uri.EscapeDataString(address)}&api_key={GOONG_API_KEY}";
                var json = await client.GetStringAsync(url);
                using var doc = JsonDocument.Parse(json);
                var loc = doc.RootElement.GetProperty("results")[0]
                    .GetProperty("geometry").GetProperty("location");
                return (loc.GetProperty("lat").GetDouble(), loc.GetProperty("lng").GetDouble());
            }
            catch { return (0, 0); }
        }

        private static (decimal gia, decimal perKm, bool apDungMin) TinhGia(string loaiXe, decimal km)
        {
            if (!_bangGia.TryGetValue(loaiXe, out var b)) b = (12_000m, 30_000m);
            var rawGia  = Math.Round(km * b.PerKm, 0);
            bool useMin = rawGia < b.ToiThieu;
            return (useMin ? b.ToiThieu : rawGia, b.PerKm, useMin);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
