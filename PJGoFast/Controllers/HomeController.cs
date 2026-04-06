using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PJGoFast.Models;
using PJGoFast.Services.Interfaces;
using System.Diagnostics;
using System.Security.Claims;
using PJGoFast.ViewModels;

namespace PJGoFast.Controllers
{
    [Authorize(Roles = "KhachHang")]
    public class HomeController : Controller
    {
        private readonly IChuyenDiService _chuyenDiService;

        private readonly ILogger _logger;

        private readonly IKhachHangService _khachHangService;
        public HomeController(IChuyenDiService chuyenDiService, IKhachHangService khachHangService)
        {
            _chuyenDiService = chuyenDiService;
            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<HomeController>();
            _khachHangService = khachHangService;
        }



        public IActionResult Index()
        {
            if(User.Identity.IsAuthenticated)
            {
                string? IdKH = User.FindFirstValue(ClaimTypes.Name);
            }
            return View();
        }

        [HttpGet]
        public IActionResult taoChuyenDi()
        {
            return View(new ChuyenDiCreateVM());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult taoChuyenDiCheck(ChuyenDiCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                @ViewBag.Error = "Vui lòng điền đầy đủ thông tin và đảm bảo dữ liệu hợp lệ.";
                return View(model);
            }
            int result = _chuyenDiService.TaoChuyenDi(model.DiemDon, model.DiemDen, model.ThoiGianDon, model.LoaiXeYeuCau, model.GhiChu, User.FindFirstValue(ClaimTypes.Name));
            if(result == 0)
            {
                @ViewBag.Success = "Tạo chuyến đi thành công!";
                return View();
            }
            else
            {
                @ViewBag.Error = "Đã xảy ra lỗi khi tạo chuyến đi. Vui lòng thử lại.";
                return View();
            }
        }


        [HttpGet]
        public IActionResult taoChuyenDi01(string diemDen)
        {
            var model = new ChuyenDiCreateVM
            {
                DiemDen = diemDen
            };
            return View("taoChuyenDi", model);
        }

















        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
