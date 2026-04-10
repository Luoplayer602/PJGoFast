using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PJGoFast.Data;
using PJGoFast.Models.Entities;
using PJGoFast.Services.Interfaces;
using PJGoFast.ViewModels;
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

        public ClaimsPrincipal? KiemTraDangNhap(string sdt, string matKhau)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.SDT == sdt.Trim());

            if (admin == null || !BCrypt.Net.BCrypt.Verify(matKhau, admin.MatKhau))
            {
                return null;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, admin.IdAdmin),
                new Claim(ClaimTypes.Name, admin.HoVaTen),
                new Claim(ClaimTypes.Role, admin.VaiTro)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(identity);
        }

        public async Task<AdminManagementIndexVM> LayDanhSachQuanLyAsync()
        {
            var admins = await _context.Admins
                .AsNoTracking()
                .OrderBy(a => a.VaiTro)
                .ThenBy(a => a.HoVaTen)
                .Select(a => new AdminListItemVM
                {
                    IdAdmin = a.IdAdmin,
                    HoVaTen = a.HoVaTen,
                    SDT = a.SDT,
                    NgaySinh = a.NgaySinh,
                    VaiTro = a.VaiTro
                })
                .ToListAsync();

            return new AdminManagementIndexVM
            {
                Admins = admins
            };
        }

        public async Task<AdminDetailsVM?> LayChiTietAsync(string idAdmin)
        {
            return await _context.Admins
                .AsNoTracking()
                .Where(a => a.IdAdmin == idAdmin)
                .Select(a => new AdminDetailsVM
                {
                    IdAdmin = a.IdAdmin,
                    HoVaTen = a.HoVaTen,
                    SDT = a.SDT,
                    NgaySinh = a.NgaySinh,
                    VaiTro = a.VaiTro
                })
                .FirstOrDefaultAsync();
        }

        public async Task<AdminEditVM?> LayAdminDeSuaAsync(string idAdmin)
        {
            return await _context.Admins
                .AsNoTracking()
                .Where(a => a.IdAdmin == idAdmin)
                .Select(a => new AdminEditVM
                {
                    IdAdmin = a.IdAdmin,
                    HoVaTen = a.HoVaTen,
                    SDT = a.SDT,
                    NgaySinh = a.NgaySinh,
                    VaiTro = a.VaiTro
                })
                .FirstOrDefaultAsync();
        }

        public async Task<(bool Success, string? ErrorMessage)> TaoAdminAsync(AdminCreateVM model)
        {
            var idAdmin = model.IdAdmin.Trim();
            var sdt = model.SDT.Trim();

            if (await _context.Admins.AnyAsync(a => a.IdAdmin == idAdmin))
            {
                return (false, "Mã admin đã tồn tại.");
            }

            if (await _context.Admins.AnyAsync(a => a.SDT == sdt))
            {
                return (false, "Số điện thoại đã được dùng cho admin khác.");
            }

            var admin = new Admin
            {
                IdAdmin = idAdmin,
                HoVaTen = model.HoVaTen.Trim(),
                SDT = sdt,
                NgaySinh = model.NgaySinh,
                VaiTro = model.VaiTro,
                MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau.Trim())
            };

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool Success, bool NotFound, string? ErrorMessage)> CapNhatAdminAsync(AdminEditVM model, string? currentAdminId)
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.IdAdmin == model.IdAdmin);
            if (admin == null)
            {
                return (false, true, null);
            }

            var sdt = model.SDT.Trim();
            if (await _context.Admins.AnyAsync(a => a.IdAdmin != model.IdAdmin && a.SDT == sdt))
            {
                return (false, false, "Số điện thoại đã được dùng cho admin khác.");
            }

            if (admin.VaiTro == AdminRoleOptions.QuanTri &&
                model.VaiTro != AdminRoleOptions.QuanTri &&
                await LaQuanTriCuoiCungAsync(model.IdAdmin))
            {
                return (false, false, "Không thể đổi vai trò của quản trị viên cuối cùng.");
            }

            admin.HoVaTen = model.HoVaTen.Trim();
            admin.SDT = sdt;
            admin.NgaySinh = model.NgaySinh;
            admin.VaiTro = model.VaiTro;

            if (model.DatLaiMatKhau)
            {
                admin.MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhauMoi!.Trim());
            }

            await _context.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(currentAdminId) &&
                currentAdminId == admin.IdAdmin &&
                admin.VaiTro != AdminRoleOptions.QuanTri)
            {
                return (true, false, "Vai trò tài khoản hiện tại đã thay đổi. Hãy đăng nhập lại nếu phiên làm việc không còn phù hợp.");
            }

            return (true, false, null);
        }

        public async Task<(bool Success, bool NotFound, string? ErrorMessage)> XoaAdminAsync(string idAdmin, string? currentAdminId)
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.IdAdmin == idAdmin);
            if (admin == null)
            {
                return (false, true, null);
            }

            if (!string.IsNullOrWhiteSpace(currentAdminId) && currentAdminId == idAdmin)
            {
                return (false, false, "Không thể tự xóa tài khoản đang đăng nhập.");
            }

            if (admin.VaiTro == AdminRoleOptions.QuanTri && await LaQuanTriCuoiCungAsync(idAdmin))
            {
                return (false, false, "Không thể xóa quản trị viên cuối cùng.");
            }

            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();

            return (true, false, null);
        }

        private async Task<bool> LaQuanTriCuoiCungAsync(string idAdmin)
        {
            var soQuanTriConLai = await _context.Admins
                .CountAsync(a => a.VaiTro == AdminRoleOptions.QuanTri && a.IdAdmin != idAdmin);

            return soQuanTriConLai == 0;
        }
    }
}
