using Microsoft.AspNetCore.Mvc;
using PJGoFast.Data;
using PJGoFast.Models.Entities;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using PJGoFast.Services.Interfaces;


namespace PJGoFast.Controllers
{

    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;

        private readonly IKhachHangService _khachHangService;

        private readonly PJGoFastDbContext _context;

        private readonly IAdminService _adminService;

        private readonly ITaiXeService _taiXeService;

        public LoginController(ILogger<LoginController> logger, IKhachHangService khachHangService, PJGoFastDbContext context, IAdminService adminService, ITaiXeService taiXeService)
        {
            _logger = logger;
            _khachHangService = khachHangService;
            _context = context;
            _adminService = adminService;
            _taiXeService = taiXeService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (User.IsInRole("KhachHang"))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string sdt, string matKhau, string ReturnUrl)
        {
            if (string.IsNullOrEmpty(sdt) || string.IsNullOrEmpty(matKhau))
            {
                ViewBag.Error = "Số điện thoại và mật khẩu không được để trống.";
                return View();
            }

            var principal = _khachHangService.KiemTraDangNhap(sdt, matKhau);

            if (principal == null)
            {
                ViewBag.Error = "Số điện thoại hoặc mật khẩu không đúng.";
                return View();
            }

            

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            //-		$exception	{"No sign-in authentication handler is registered for the scheme 'CookieAuth'. The registered sign-in schemes are: Cookies. Did you forget to call AddAuthentication().AddCookie(\"CookieAuth\",...)?"}	System.InvalidOperationException


            if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult register()
        {
            if (User.IsInRole("KhachHang"))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult register(string sdt, string matKhau, string confirmMatKhau)
        {
            int result = _khachHangService.DangKy(sdt, matKhau, confirmMatKhau);
            if (result == 1)
            {
                ViewBag.Error = "Số điện thoại đã tồn tại.";
                return View();
            }
            else if (result == 2)
            {
                ViewBag.Error = "Đã xảy ra lỗi khi lưu dữ liệu.";
                return View();
            }
            else if (result == 3)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp.";
                return View();
            }
            else
            {
                ViewBag.Success = "Đăng ký thành công. Bạn có thể đăng nhập ngay bây giờ.";
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpGet]
        public IActionResult Admin()
        {
            if (User.IsInRole("QuanTri"))
            {
                return RedirectToAction("AdminMenu", "Admin");
            }
            else if (User.IsInRole("DieuPhoi"))
            {
                return RedirectToAction("Index", "DieuPhoi");
            }
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Admin(string sdt, string matKhau, string ReturnUrl)
        {
            if (string.IsNullOrEmpty(sdt) || string.IsNullOrEmpty(matKhau))
            {
                ViewBag.Error = "Số điện thoại và mật khẩu không được để trống.";
                return View();
            }

            var principal = _adminService.KiemTraDangNhap(sdt, matKhau);

            if (principal == null)
            {
                ViewBag.Error = "Số điện thoại hoặc mật khẩu không đúng.";
                return View();
            }

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }

            if (principal.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "QuanTri"))
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "DieuPhoi");
            }    
        }


        public IActionResult TaiXe()
        {
            if (User.IsInRole("TaiXe"))
            {
                return RedirectToAction("Index", "TaiXe");
            }
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaiXe(string sdt, string matKhau)
        {
            var principal = _taiXeService.KiemTraDangNhap(sdt, matKhau);

            if (principal == null)
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
                return View();
            }

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "TaiXe"); // chuyển về trang tài xế
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index));
        }



    }
}
