using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PJGoFast.Data;

namespace PJGoFast.Controllers
{
    public class AdminController : Controller
    {
        




        public IActionResult Index()
        {
            return View();
        }


        [Authorize(Roles = "QuanTri")]
        public IActionResult QuanLyAdmin()
        {
            return RedirectToAction("Index", "QuanLyAdmins");
        }




    }
}
