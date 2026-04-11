using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PJGoFast.Services.Interfaces;
using PJGoFast.ViewModels;
using System.Security.Claims;

namespace PJGoFast.Controllers
{
    [Authorize(Roles = "QuanTri")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> Index()
        {
            var vm = await _adminService.LayDanhSachQuanLyAsync();
            return View(vm);
        }

        public IActionResult QuanLyAdmin()
        {
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {
            return View(new AdminCreateVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _adminService.TaoAdminAsync(model);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Không thể tạo admin.");
                return View(model);
            }

            TempData["Success"] = "Đã tạo admin mới.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var vm = await _adminService.LayChiTietAsync(id);
            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var vm = await _adminService.LayAdminDeSuaAsync(id);
            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminEditVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentAdminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _adminService.CapNhatAdminAsync(model, currentAdminId);
            if (result.NotFound)
            {
                return NotFound();
            }

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Không thể cập nhật admin.");
                return View(model);
            }

            TempData["Success"] = result.ErrorMessage ?? "Đã cập nhật thông tin admin.";
            return RedirectToAction(nameof(Details), new { id = model.IdAdmin });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var currentAdminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _adminService.XoaAdminAsync(id, currentAdminId);
            if (result.NotFound)
            {
                return NotFound();
            }

            if (!result.Success)
            {
                TempData["Error"] = result.ErrorMessage ?? "Không thể xóa admin.";
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["Success"] = "Đã xóa admin.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult AdminMenu()
        {
            return View();
        }

    }
}
