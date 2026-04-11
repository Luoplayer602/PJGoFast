using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PJGoFast.Models.Enums;
using PJGoFast.Services.Interfaces;
using PJGoFast.ViewModels;
using System.Security.Claims;

namespace PJGoFast.Controllers
{
    [Authorize(Roles = "TaiXe")]
    public class TaiXeController : Controller
    {
        private readonly IChuyenDiService _chuyenDiService;
        private readonly ITaiXeService _taiXeService;

        public TaiXeController(IChuyenDiService chuyenDiService, ITaiXeService taiXeService)
        {
            _chuyenDiService = chuyenDiService;
            _taiXeService = taiXeService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var vm = _chuyenDiService.LayBangDieuKhienTaiXe(GetCurrentDriverId());
            return vm == null ? NotFound() : View(vm);
        }

        [HttpGet]
        public IActionResult DashboardData()
        {
            var vm = _chuyenDiService.LayBangDieuKhienTaiXe(GetCurrentDriverId());
            return Json(new { success = vm != null, data = vm });
        }

        [HttpGet]
        public IActionResult ChiTietChuyen(string idChuyenDi)
        {
            var trip = _chuyenDiService.LayChiTietChuyen(idChuyenDi);
            if (trip == null)
            {
                return NotFound();
            }

            var idTX = GetCurrentDriverId();
            var duocXem = trip.TrangThai == TrangThaiChuyen.MOI || trip.IdTX == idTX || trip.TrangThai == TrangThaiChuyen.DA_PHAN_CONG;
            return duocXem ? View(trip) : Forbid();
        }

        [HttpGet]
        public IActionResult ChiTietChuyenData(string idChuyenDi)
        {
            var trip = _chuyenDiService.LayChiTietChuyen(idChuyenDi);
            if (trip == null)
            {
                return Json(new { success = false, message = "Không tìm thấy chuyến đi." });
            }

            var idTX = GetCurrentDriverId();
            var duocXem = trip.TrangThai == TrangThaiChuyen.MOI || trip.IdTX == idTX || trip.TrangThai == TrangThaiChuyen.DA_PHAN_CONG;
            return Json(new { success = duocXem, data = duocXem ? trip : null });
        }

        [HttpGet]
        public IActionResult TienTrinh(string idChuyenDi)
        {
            var trip = _chuyenDiService.LayChiTietChuyen(idChuyenDi);
            if (trip == null || trip.IdTX != GetCurrentDriverId())
            {
                return NotFound();
            }

            return View(trip);
        }

        [HttpGet]
        public IActionResult TienTrinhData(string idChuyenDi)
        {
            var trip = _chuyenDiService.LayChiTietChuyen(idChuyenDi);
            var success = trip != null && trip.IdTX == GetCurrentDriverId();
            return Json(new { success, data = success ? trip : null });
        }

        [HttpGet]
        public IActionResult Account()
        {
            var vm = _chuyenDiService.LayThongTinTaiKhoanTaiXe(GetCurrentDriverId());
            return vm == null ? NotFound() : View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThaiOnline(TrangThaiOnline trangThai)
        {
            var result = await _taiXeService.CapNhatTrangThaiOnlineAsync(GetCurrentDriverId(), trangThai);
            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NhanChuyen(string idChuyenDi)
        {
            var result = _chuyenDiService.NhanChuyen(idChuyenDi, GetCurrentDriverId());
            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return result.Success && !string.IsNullOrWhiteSpace(result.RedirectTripId)
                ? RedirectToAction(nameof(TienTrinh), new { idChuyenDi = result.RedirectTripId })
                : RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TuChoiPhanCong(string idChuyenDi)
        {
            var result = _chuyenDiService.TuChoiChuyenDuocPhanCong(idChuyenDi, GetCurrentDriverId());
            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CapNhatTienTrinh(string idChuyenDi, TrangThaiChuyen trangThaiMoi)
        {
            var result = _chuyenDiService.CapNhatTienTrinhTaiXe(idChuyenDi, GetCurrentDriverId(), trangThaiMoi);
            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(TienTrinh), new { idChuyenDi });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult HuyChuyen(string idChuyenDi)
        {
            var result = _chuyenDiService.HuyChuyenTuTaiXe(idChuyenDi, GetCurrentDriverId());
            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XacNhanThanhToan(XacNhanThanhToanVM model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Thông tin thanh toán không hợp lệ.";
                return RedirectToAction(nameof(TienTrinh), new { idChuyenDi = model.IdChuyenDi });
            }

            var result = _chuyenDiService.XacNhanThanhToan(
                model.IdChuyenDi,
                GetCurrentDriverId(),
                model.PhuongThucThanhToan,
                model.SoTienThanhToan);

            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(TienTrinh), new { idChuyenDi = model.IdChuyenDi });
        }

        private string GetCurrentDriverId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }
    }
}
