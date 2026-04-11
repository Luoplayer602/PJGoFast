using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PJGoFast.Services.Interfaces;
using PJGoFast.ViewModels;

namespace PJGoFast.Controllers
{
    [Authorize(Roles = "QuanTri")]
    public class TaiXeMNGController : Controller
    {
        private readonly ITaiXeService _taiXeService;

        public TaiXeMNGController(ITaiXeService taiXeService)
        {
            _taiXeService = taiXeService;
        }

        // ─── Danh sách tài xế ─────────────────────────────────────────────────
        public async Task<IActionResult> IndexTX()
        {
            var vm = await _taiXeService.LayDanhSachAsync();
            return View(vm);
        }

        // ─── Chi tiết ─────────────────────────────────────────────────────────
        public async Task<IActionResult> DetailsTX(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            var vm = await _taiXeService.LayChiTietAsync(id);
            if (vm == null) return NotFound();

            return View(vm);
        }

        // ─── Tạo mới ──────────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult CreateTX() => View(new TaiXeCreateVM());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTX(TaiXeCreateVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _taiXeService.TaoTaiXeAsync(model);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Không thể tạo tài xế.");
                return View(model);
            }

            TempData["Success"] = "Đã tạo tài xế mới thành công.";
            return RedirectToAction(nameof(IndexTX));
        }

        // ─── Sửa ──────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> EditTX(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            var vm = await _taiXeService.LayTaiXeDeSuaAsync(id);
            if (vm == null) return NotFound();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTX(TaiXeEditVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _taiXeService.CapNhatTaiXeAsync(model);
            if (result.NotFound) return NotFound();

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Không thể cập nhật tài xế.");
                return View(model);
            }

            TempData["Success"] = result.ErrorMessage ?? "Đã cập nhật thông tin tài xế.";
            return RedirectToAction(nameof(DetailsTX), new { id = model.IdTX });
        }

        // ─── Xóa ──────────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTX(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            var result = await _taiXeService.XoaTaiXeAsync(id);
            if (result.NotFound) return NotFound();

            if (!result.Success)
            {
                TempData["Error"] = result.ErrorMessage ?? "Không thể xóa tài xế.";
                return RedirectToAction(nameof(DetailsTX), new { id });
            }

            TempData["Success"] = "Đã xóa tài xế.";
            return RedirectToAction(nameof(IndexTX));
        }
    }
}
