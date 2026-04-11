using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PJGoFast.Services.Interfaces;
using System.Security.Claims;

namespace PJGoFast.Controllers
{
    [Authorize(Roles = "DieuPhoi,QuanTri")]
    public class DieuPhoiController : Controller
    {
        private readonly IChuyenDiService _chuyenDiService;
        private readonly ITaiXeService _taiXeService;

        public DieuPhoiController(IChuyenDiService chuyenDiService, ITaiXeService taiXeService)
        {
            _chuyenDiService = chuyenDiService;
            _taiXeService = taiXeService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var vm = _chuyenDiService.LayBangDieuPhoi(GetCurrentAdminId());
            return vm == null ? NotFound() : View(vm);
        }

        [HttpGet]
        public IActionResult DashboardData()
        {
            var vm = _chuyenDiService.LayBangDieuPhoi(GetCurrentAdminId());
            return Json(new { success = vm != null, data = vm });
        }

        [HttpGet]
        public IActionResult ChiTietChuyenData(string idChuyenDi)
        {
            var trip = _chuyenDiService.LayChiTietChuyen(idChuyenDi);
            return Json(new { success = trip != null, data = trip });
        }

        [HttpGet]
        public async Task<IActionResult> ChiTietTaiXeData(string idTX)
        {
            var detail = await _taiXeService.LayChiTietAsync(idTX);
            if (detail == null)
            {
                return Json(new { success = false, message = "Không tìm thấy tài xế." });
            }

            var dashboard = _chuyenDiService.LayBangDieuPhoi(GetCurrentAdminId());
            var driver = dashboard?.TaiXes.FirstOrDefault(t => t.IdTX == idTX);
            return Json(new { success = true, data = new { detail, summary = driver } });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PhanCong(string idChuyenDi, string idTX)
        {
            var result = _chuyenDiService.PhanCongChuyen(idChuyenDi, idTX, GetCurrentAdminId());
            return Json(new { success = result.Success, message = result.Message });
        }

        private string GetCurrentAdminId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }
    }
}
