using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PJGoFast.Models;
using PJGoFast.Services.Interfaces;
using PJGoFast.ViewModels;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

// NOTE: Đảm bảo đã thêm dòng sau vào Program.cs:
//   builder.Services.AddHttpClient();

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
        // Format: { "TênLoaiXe", (GiáMỗiKm, GiáTốiThiểu) }
        private static readonly Dictionary<string, (decimal PerKm, decimal ToiThieu)> _bangGia = new()
        {
            { "XeMay",   (  8_000m,  15_000m) },   // Xe máy  :  8.000 đ/km, tối thiểu  15.000 đ
            { "Sedan",   ( 12_000m,  30_000m) },   // Sedan   : 12.000 đ/km, tối thiểu  30.000 đ
            { "SUV",     ( 15_000m,  40_000m) },   // SUV     : 15.000 đ/km, tối thiểu  40.000 đ
            { "BayCho",  ( 18_000m,  50_000m) },   // 7 chỗ  : 18.000 đ/km, tối thiểu  50.000 đ
            { "TaiNho",  ( 22_000m,  60_000m) },   // Tải nhỏ: 22.000 đ/km, tối thiểu  60.000 đ
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

        // ─── Lịch sử chuyến đi ───────────────────────────────────────────────
        [HttpGet]
        public IActionResult History()
        {
            var idKhachHang = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrWhiteSpace(idKhachHang))
            {
                return Challenge();
            }

            var lichSu = _chuyenDiService.LayLichSuChuyenDiTheoKhachHang(idKhachHang)
                .Select(cd => new ChuyenDiHistoryItemVM
                {
                    IdChuyenDi = cd.IdChuyenDi,
                    DiemDon = cd.DiemDon,
                    DiemDen = cd.DiemDen,
                    LoaiXeYeuCau = cd.LoaiXeYeuCau,
                    TrangThai = cd.TrangThai,
                    ThoiGianTao = cd.ThoiGianTao,
                    ThoiGianDon = cd.ThoiGianDon,
                    GiaTamTinh = cd.GiaTamTinh
                })
                .ToList();

            return View(lichSu);
        }

        [HttpGet]
        public IActionResult Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var idKhachHang = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrWhiteSpace(idKhachHang))
            {
                return Challenge();
            }

            var chuyenDi = _chuyenDiService.LayChiTietChuyenDiCuaKhachHang(id, idKhachHang);
            if (chuyenDi == null)
            {
                return NotFound();
            }

            var model = new ChuyenDiHistoryDetailsVM
            {
                IdChuyenDi = chuyenDi.IdChuyenDi,
                DiemDon = chuyenDi.DiemDon,
                DiemDen = chuyenDi.DiemDen,
                LoaiXeYeuCau = chuyenDi.LoaiXeYeuCau,
                TrangThai = chuyenDi.TrangThai,
                ThoiGianTao = chuyenDi.ThoiGianTao,
                ThoiGianDon = chuyenDi.ThoiGianDon,
                GhiChu = chuyenDi.GhiChu,
                GiaTamTinh = chuyenDi.GiaTamTinh,
                GiaThucTe = chuyenDi.GiaThucTe,
                NhatKys = (chuyenDi.NhatKys ?? Enumerable.Empty<Models.Entities.NhatKy>())
                    .OrderByDescending(nk => nk.ThoiGian)
                    .Select(nk => new NhatKyVM
                    {
                        IdNhatKy = nk.IdNhatKy,
                        TrangThaiCu = nk.TrangThaiCu,
                        TrangThaiMoi = nk.TrangThaiMoi,
                        ThucHienBoi = nk.ThucHienBoi,
                        ThoiGian = nk.ThoiGian,
                        LogText = nk.LogText
                    })
                    .ToList()
            };

            return View(model);
        }

        // Giữ tương thích với route cũ bị gõ nhầm "Histoty"
        [HttpGet]
        public IActionResult Histoty() => RedirectToAction(nameof(History));

        [HttpGet]
        public IActionResult Account()
        {
            var idKhachHang = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrWhiteSpace(idKhachHang))
            {
                return Challenge();
            }

            var khachHang = _khachHangService.GetKhachHangById(idKhachHang);
            if (khachHang == null)
            {
                return NotFound();
            }

            var model = new AccountVM
            {
                IdKH = khachHang.IdKH,
                HoVaTen = khachHang.HoVaTen,
                SDT = khachHang.SDT,
                Email = khachHang.Email,
                NgaySinh = khachHang.NgaySinh,
                NgayDangKy = khachHang.NgayDangKy
            };

            return View(model);
        }

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

        // ─── Báo giá (basic view) ─────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> baoGia(string IdChuyenDi)
        {
            var vm = await BuildBaoGiaVMAsync(IdChuyenDi);
            if (vm == null) return NotFound();

            // Lưu giá tạm tính vào DB
            _chuyenDiService.CapNhatGiaTamTinh(IdChuyenDi, vm.GiaTamTinh);

            return View(vm);
        }

        // ─── Báo giá (premium map view) ───────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> baoGiaMap(string IdChuyenDi)
        {
            var vm = await BuildBaoGiaVMAsync(IdChuyenDi);
            if (vm == null) return NotFound();

            vm.GoongMapKey = GOONG_MAP_KEY;
            vm.GoongApiKey = GOONG_API_KEY;

            return View(vm);
        }

        // ─── Helpers ─────────────────────────────────────────────────────────

        /// <summary>Dựng BaoGiaVM: geocode 2 địa chỉ → gọi distance matrix → tính giá.</summary>
        private async Task<BaoGiaVM?> BuildBaoGiaVMAsync(string idChuyenDi)
        {
            var chuyenDi = _chuyenDiService.LayChuyenDiTheoId(idChuyenDi);
            if (chuyenDi == null) return null;

            var client = _httpClientFactory.CreateClient();

            // Geocode song song để tiết kiệm thời gian
            var taskDon = GeocodeAsync(client, chuyenDi.DiemDon);
            var taskDen = GeocodeAsync(client, chuyenDi.DiemDen);
            await Task.WhenAll(taskDon, taskDen);

            var (donLat, donLng) = taskDon.Result;
            var (denLat, denLng) = taskDen.Result;

            double khoangCachKm = 0;
            int thoiGianPhut = 0;

            if (donLat != 0 && denLat != 0)
            {
                // Gọi Distance Matrix API
                var dmUrl = $"https://rsapi.goong.io/v2/distancematrix" +
                            $"?origins={donLat.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                            $",{donLng.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                            $"&destinations={denLat.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                            $",{denLng.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                            $"&vehicle=car&api_key={GOONG_API_KEY}";

                try
                {
                    var dmJson = await client.GetStringAsync(dmUrl);
                    using var dmDoc = JsonDocument.Parse(dmJson);
                    var element = dmDoc.RootElement
                        .GetProperty("rows")[0]
                        .GetProperty("elements")[0];

                    if (element.GetProperty("status").GetString() == "OK")
                    {
                        int meters  = element.GetProperty("distance").GetProperty("value").GetInt32();
                        int seconds = element.GetProperty("duration").GetProperty("value").GetInt32();
                        khoangCachKm = Math.Round(meters  / 1000.0, 1);
                        thoiGianPhut = (int)Math.Ceiling(seconds / 60.0);
                    }
                }
                catch { /* Giữ mặc định 0 nếu API lỗi */ }
            }

            var (giaTamTinh, giaPerKm, apDungMin) = TinhGia(
                chuyenDi.LoaiXeYeuCau.ToString(), (decimal)khoangCachKm);

            return new BaoGiaVM
            {
                IdChuyenDi       = idChuyenDi,
                DiemDon          = chuyenDi.DiemDon,
                DiemDen          = chuyenDi.DiemDen,
                ThoiGianDon      = chuyenDi.ThoiGianDon,
                LoaiXeYeuCau     = chuyenDi.LoaiXeYeuCau,
                GhiChu           = chuyenDi.GhiChu,
                KhoangCachKm     = khoangCachKm,
                ThoiGianDuTinhPhut = thoiGianPhut,
                GiaTamTinh       = giaTamTinh,
                GiaPerKm         = giaPerKm,
                ApDungGiaToiThieu = apDungMin,
                DonLat = donLat, DonLng = donLng,
                DenLat = denLat, DenLng = denLng,
            };
        }

        /// <summary>Geocode địa chỉ văn bản → (lat, lng). Trả về (0,0) nếu thất bại.</summary>
        private async Task<(double lat, double lng)> GeocodeAsync(HttpClient client, string address)
        {
            try
            {
                var url  = $"https://rsapi.goong.io/v2/geocode?address={Uri.EscapeDataString(address)}&api_key={GOONG_API_KEY}";
                var json = await client.GetStringAsync(url);
                using var doc = JsonDocument.Parse(json);
                var loc = doc.RootElement
                    .GetProperty("results")[0]
                    .GetProperty("geometry")
                    .GetProperty("location");
                return (loc.GetProperty("lat").GetDouble(), loc.GetProperty("lng").GetDouble());
            }
            catch { return (0, 0); }
        }

        /// <summary>Tính giá tạm tính theo loại xe và số km.</summary>
        private static (decimal gia, decimal perKm, bool apDungMin) TinhGia(string loaiXe, decimal km)
        {
            if (!_bangGia.TryGetValue(loaiXe, out var b))
                b = (12_000m, 30_000m); // fallback: Sedan

            var rawGia = Math.Round(km * b.PerKm, 0);
            bool useMin = rawGia < b.ToiThieu;
            return (useMin ? b.ToiThieu : rawGia, b.PerKm, useMin);
        }

        // ─── Misc ─────────────────────────────────────────────────────────────
        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
