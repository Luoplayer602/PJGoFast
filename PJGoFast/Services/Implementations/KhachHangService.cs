using PJGoFast.Data;
using PJGoFast.Services.Interfaces;
using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Azure.Core;

namespace PJGoFast.Services.Implementations
{
    public class KhachHangService : IKhachHangService
    {
        private readonly PJGoFastDbContext _context;

        

        public KhachHangService(PJGoFastDbContext context)
        {
            _context = context;

        }


        public int DangKy(string sdt, string matKhau, string confirmMatKhau)
        {
            bool isPhoneExist = _context.KhachHangs.Any(kh => kh.SDT == sdt.Trim());
            if (isPhoneExist)
            {
                return 1; // Số điện thoại đã tồn tại
            }
            else if (matKhau != confirmMatKhau)
            {
                return 3; // Mật khẩu xác nhận không khớp
            }

            var newKH = new Models.Entities.KhachHang
            {
                IdKH = Guid.NewGuid().ToString(),
                SDT = sdt.Trim(),
                HoVaTen = "Khách hàng mới",
                NgayDangKy = DateTime.Now,
                MatKhau = BCrypt.Net.BCrypt.HashPassword(matKhau)
            };

            try
            {
                _context.KhachHangs.Add(newKH);
                _context.SaveChanges();
                return 0; // Đăng ký thành công
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần thiết
                return 2; // Lỗi khi lưu dữ liệu
            }
        }

        public ClaimsPrincipal KiemTraDangNhap(string sdt, string matKhau)
        {
            var KhachHang = _context.KhachHangs.FirstOrDefault(kh => kh.SDT == sdt.Trim());

            if (KhachHang == null)
            {
                return null; // Số điện thoại không tồn tại
            }
            else if (!BCrypt.Net.BCrypt.Verify(matKhau, KhachHang.MatKhau))
            {
                return null; // Mật khẩu không đúng
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, KhachHang.IdKH),
                new Claim(ClaimTypes.Name, KhachHang.IdKH),
                new Claim(ClaimTypes.MobilePhone, KhachHang.SDT),
                new Claim(ClaimTypes.Role, "KhachHang"),
                new Claim("IdKH", KhachHang.IdKH)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            return principal; // Đăng nhập thành công
        }

        public Models.Entities.KhachHang LayThongTinKhachHang(string idKH)
        {
            return _context.KhachHangs.FirstOrDefault(kh => kh.IdKH == idKH);

        }

        
    }
}
