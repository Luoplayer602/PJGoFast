using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PJGoFast.Models;
using PJGoFast.Services.Interfaces;
using PJGoFast.ViewModels;
using System.Diagnostics;
using System.Security.Claims;

namespace PJGoFast.Controllers
{
    [Authorize(Roles = "KhachHang")]
    public class HomeController : Controller
    {
        private readonly IChuyenDiService _chuyenDiService;

        

        private readonly IKhachHangService _khachHangService;
        public HomeController(IChuyenDiService chuyenDiService, IKhachHangService khachHangService)
        {
            _chuyenDiService = chuyenDiService;
            
            _khachHangService = khachHangService;
        }



        public IActionResult Index()
        {
            
            return View();
        }

        [HttpGet]
        public IActionResult taoChuyenDi()
        {
            var model = new ChuyenDiCreateVM()
            {
                IdKH = User.FindFirstValue(ClaimTypes.Name)

            };



            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult taoChuyenDi(ChuyenDiCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Vui lòng điền đầy đủ thông tin và đảm bảo dữ liệu hợp lệ.";
                return View(model);
            }
            int result = _chuyenDiService.TaoChuyenDi(model.DiemDon, model.DiemDen, model.ThoiGianDon, model.LoaiXeYeuCau, model.GhiChu, model.IdKH);
            if (result == 0)
            {
                ViewBag.Success = "Tạo chuyến đi thành công!";
                return View();
            }
            else
            {
                ViewBag.Error = "Đã xảy ra lỗi khi tạo chuyến đi. Vui lòng thử lại.";
                return View();
            }
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
