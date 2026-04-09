using Microsoft.AspNetCore.Authentication.Cookies;
using PJGoFast.Data;
using PJGoFast.Models.Entities;
using PJGoFast.Services.Interfaces;
using System.Security.Claims;

namespace PJGoFast.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly PJGoFastDbContext _context;

        public AdminService(PJGoFastDbContext context)
        {
            _context = context;
        }

        public ClaimsPrincipal KiemTraDangNhap(string sdt, string matKhau)
        {
            var Admin = _context.Admins.FirstOrDefault(a => a.SDT == sdt.Trim());

            if (Admin == null)
            {
                return null; // Số điện thoại không tồn tại
            }
            else if (!BCrypt.Net.BCrypt.Verify(matKhau, Admin.MatKhau))
            {
                return null; // Mật khẩu không đúng
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Admin.IdAdmin),
                new Claim(ClaimTypes.Name, Admin.HoVaTen),
                new Claim(ClaimTypes.Role, Admin.VaiTro) 
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            return principal;
        }
    }
}
