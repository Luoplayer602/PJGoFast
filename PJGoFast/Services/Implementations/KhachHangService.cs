using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PJGoFast.Data;
using PJGoFast.Models.Entities;
using PJGoFast.Models.Enums;
using PJGoFast.Services.Interfaces;
using PJGoFast.ViewModels;
using System.Security.Claims;

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
            var phone = sdt.Trim();
            if (_context.KhachHangs.Any(kh => kh.SDT == phone))
            {
                return 1;
            }

            if (matKhau != confirmMatKhau)
            {
                return 3;
            }

            var khachHang = new KhachHang
            {
                IdKH = Guid.NewGuid().ToString(),
                SDT = phone,
                HoVaTen = "Khách hàng mới",
                NgayDangKy = DateTime.Now,
                MatKhau = BCrypt.Net.BCrypt.HashPassword(matKhau)
            };

            try
            {
                _context.KhachHangs.Add(khachHang);
                _context.SaveChanges();
                return 0;
            }
            catch
            {
                return 2;
            }
        }

        public ClaimsPrincipal KiemTraDangNhap(string sdt, string matKhau)
        {
            var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.SDT == sdt.Trim());
            if (khachHang == null || !BCrypt.Net.BCrypt.Verify(matKhau, khachHang.MatKhau))
            {
                return null;
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, khachHang.IdKH),
                new(ClaimTypes.Name, khachHang.IdKH),
                new(ClaimTypes.MobilePhone, khachHang.SDT),
                new(ClaimTypes.Role, "KhachHang"),
                new("IdKH", khachHang.IdKH)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(identity);
        }

        public KhachHang LayThongTinKhachHang(string idKH)
        {
            return _context.KhachHangs.FirstOrDefault(kh => kh.IdKH == idKH);
        }

        public KhachHangAccountPageVM? LayTrangTaiKhoan(string idKH)
        {
            var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.IdKH == idKH);
            if (khachHang == null)
            {
                return null;
            }

            return new KhachHangAccountPageVM
            {
                KhachHang = khachHang,
                ProfileForm = new KhachHangProfileUpdateVM
                {
                    HoVaTen = khachHang.HoVaTen,
                    Email = khachHang.Email,
                    NgaySinh = khachHang.NgaySinh
                }
            };
        }

        public (bool Success, string Message) CapNhatThongTin(string idKH, KhachHangProfileUpdateVM model)
        {
            var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.IdKH == idKH);
            if (khachHang == null)
            {
                return (false, "Không tìm thấy tài khoản.");
            }

            khachHang.HoVaTen = model.HoVaTen.Trim();
            khachHang.Email = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email.Trim();
            khachHang.NgaySinh = model.NgaySinh;
            _context.SaveChanges();
            return (true, "Đã cập nhật thông tin tài khoản.");
        }

        public (bool Success, string Message) DoiMatKhau(string idKH, KhachHangChangePasswordVM model)
        {
            var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.IdKH == idKH);
            if (khachHang == null)
            {
                return (false, "Không tìm thấy tài khoản.");
            }

            if (!BCrypt.Net.BCrypt.Verify(model.MatKhauHienTai, khachHang.MatKhau))
            {
                return (false, "Mật khẩu hiện tại không đúng.");
            }

            khachHang.MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhauMoi.Trim());
            _context.SaveChanges();
            return (true, "Đã đổi mật khẩu.");
        }

        public async Task<(bool Success, string Message)> XoaTaiKhoanAsync(string idKH, string matKhauXacNhan)
        {
            var khachHang = await _context.KhachHangs
                .Include(k => k.ChuyenDis)
                .ThenInclude(c => c.NhatKys)
                .Include(k => k.ChuyenDis)
                .ThenInclude(c => c.ThanhToan)
                .FirstOrDefaultAsync(k => k.IdKH == idKH);

            if (khachHang == null)
            {
                return (false, "Không tìm thấy tài khoản.");
            }

            if (!BCrypt.Net.BCrypt.Verify(matKhauXacNhan, khachHang.MatKhau))
            {
                return (false, "Mật khẩu xác nhận không đúng.");
            }

            var coChuyenDangMo = khachHang.ChuyenDis.Any(c => c.TrangThai != TrangThaiChuyen.HUY && c.TrangThai != TrangThaiChuyen.HOAN_TAT);
            if (coChuyenDangMo)
            {
                return (false, "Tài khoản đang có chuyến chưa kết thúc. Không thể xóa.");
            }

            foreach (var trip in khachHang.ChuyenDis.ToList())
            {
                if (trip.ThanhToan != null)
                {
                    _context.ThanhToans.Remove(trip.ThanhToan);
                }

                if (trip.NhatKys.Any())
                {
                    _context.nhatKys.RemoveRange(trip.NhatKys);
                }

                _context.ChuyenDis.Remove(trip);
            }

            _context.KhachHangs.Remove(khachHang);
            await _context.SaveChangesAsync();
            return (true, "Đã xóa tài khoản.");
        }
    }
}
