using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PJGoFast.Controllers
{
    [Authorize(Roles = "QuanTri")]
    public class QuanLyAdminsController : Controller
    {
        public IActionResult Index() => RedirectToAction("Index", "Admin");
        public IActionResult Create() => RedirectToAction("Create", "Admin");
        public IActionResult Details(string id) => RedirectToAction("Details", "Admin", new { id });
        public IActionResult Edit(string id) => RedirectToAction("Edit", "Admin", new { id });
        public IActionResult Delete(string id) => RedirectToAction("Details", "Admin", new { id });
    }
}
