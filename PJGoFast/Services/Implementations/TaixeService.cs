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
    public class TaiXeService : ITaiXeService
    {
        private readonly PJGoFastDbContext _context;

        public TaiXeService(PJGoFastDbContext context) => _context = context;

        // ──────────────────────────────────────────────────────────────────────
        // Đăng nhập (giữ nguyên logic cũ)
        // ──────────────────────────────────────────────────────────────────────
        public ClaimsPrincipal KiemTraDangNhap(string sdt, string matKhau)
        {
            var taiXe = _context.TaiXes.FirstOrDefault(t => t.SDT == sdt.Trim());
            if (taiXe == null) return null;
            if (!BCrypt.Net.BCrypt.Verify(matKhau, taiXe.MatKhau)) return null;

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, taiXe.IdTX),
                new(ClaimTypes.Name,           taiXe.HoVaTen),
                new(ClaimTypes.Role,           "TaiXe"),
            };
            var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(identity);
        }

        // ──────────────────────────────────────────────────────────────────────
        // Danh sách
        // ──────────────────────────────────────────────────────────────────────
        public async Task<TaiXeManagementIndexVM> LayDanhSachAsync()
        {
            var list = await _context.TaiXes
                .OrderBy(t => t.HoVaTen)
                .Select(t => new TaiXeRowVM
                {
                    IdTX              = t.IdTX,
                    HoVaTen           = t.HoVaTen,
                    SDT               = t.SDT,
                    NgaySinh          = t.NgaySinh,
                    TrangThaiHoatDong = t.TrangThaiHoatDong,
                    TrangThaiOnline   = t.TrangThaiOnline.ToString(),
                    LoaiXe            = t.LoaiXe.ToString(),   // Display name ánh xạ ở View
                    DiemDoi           = t.DiemDoi,
                })
                .ToListAsync();

            return new TaiXeManagementIndexVM { TaiXes = list };
        }

        // ──────────────────────────────────────────────────────────────────────
        // Chi tiết
        // ──────────────────────────────────────────────────────────────────────
        public async Task<TaiXeDetailsVM?> LayChiTietAsync(string idTX)
        {
            var t = await _context.TaiXes
                .Include(x => x.ChuyenDis)
                .FirstOrDefaultAsync(x => x.IdTX == idTX);
            if (t == null) return null;

            return new TaiXeDetailsVM
            {
                IdTX              = t.IdTX,
                HoVaTen           = t.HoVaTen,
                SDT               = t.SDT,
                NgaySinh          = t.NgaySinh,
                TrangThaiHoatDong = t.TrangThaiHoatDong,
                TrangThaiOnline   = t.TrangThaiOnline.ToString(),
                LoaiXe            = t.LoaiXe,
                DiemDoi           = t.DiemDoi,
                ViTri             = t.ViTri,
                SoChuyenDaLam     = t.ChuyenDis?.Count ?? 0,
            };
        }

        // ──────────────────────────────────────────────────────────────────────
        // Lấy ViewModel để sửa
        // ──────────────────────────────────────────────────────────────────────
        public async Task<TaiXeEditVM?> LayTaiXeDeSuaAsync(string idTX)
        {
            var t = await _context.TaiXes.FindAsync(idTX);
            if (t == null) return null;

            return new TaiXeEditVM
            {
                IdTX              = t.IdTX,
                HoVaTen           = t.HoVaTen,
                SDT               = t.SDT,
                NgaySinh          = t.NgaySinh,
                LoaiXe            = t.LoaiXe,
                DiemDoi           = t.DiemDoi,
                TrangThaiHoatDong = t.TrangThaiHoatDong,
            };
        }

        // ──────────────────────────────────────────────────────────────────────
        // Tạo mới
        // ──────────────────────────────────────────────────────────────────────
        public async Task<TaiXeServiceResult> TaoTaiXeAsync(TaiXeCreateVM model)
        {
            // Kiểm tra trùng IdTX
            if (await _context.TaiXes.AnyAsync(t => t.IdTX == model.IdTX))
                return TaiXeServiceResult.Fail($"Mã tài xế '{model.IdTX}' đã tồn tại.");

            // Kiểm tra trùng SĐT
            if (await _context.TaiXes.AnyAsync(t => t.SDT == model.SDT.Trim()))
                return TaiXeServiceResult.Fail($"Số điện thoại '{model.SDT}' đã được đăng ký.");

            var taiXe = new TaiXe
            {
                IdTX              = model.IdTX.Trim().ToUpper(),
                HoVaTen           = model.HoVaTen.Trim(),
                SDT               = model.SDT.Trim(),
                NgaySinh          = model.NgaySinh,
                LoaiXe            = model.LoaiXe,
                DiemDoi           = model.DiemDoi?.Trim(),
                TrangThaiHoatDong = "HoatDong",
                TrangThaiOnline   = TrangThaiOnline.OFFLINE,
                MatKhau           = BCrypt.Net.BCrypt.HashPassword(model.MatKhau),
            };

            try
            {
                _context.TaiXes.Add(taiXe);
                await _context.SaveChangesAsync();
                return TaiXeServiceResult.Ok("Đã tạo tài xế thành công.");
            }
            catch (Exception ex)
            {
                return TaiXeServiceResult.Fail("Lỗi khi lưu dữ liệu: " + ex.Message);
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        // Cập nhật
        // ──────────────────────────────────────────────────────────────────────
        public async Task<TaiXeServiceResult> CapNhatTaiXeAsync(TaiXeEditVM model)
        {
            var taiXe = await _context.TaiXes.FindAsync(model.IdTX);
            if (taiXe == null) return TaiXeServiceResult.Miss();

            // Kiểm tra trùng SĐT (trừ chính tài xế này)
            if (await _context.TaiXes.AnyAsync(t => t.SDT == model.SDT.Trim() && t.IdTX != model.IdTX))
                return TaiXeServiceResult.Fail($"Số điện thoại '{model.SDT}' đã được đăng ký bởi tài xế khác.");

            taiXe.HoVaTen           = model.HoVaTen.Trim();
            taiXe.SDT               = model.SDT.Trim();
            taiXe.NgaySinh          = model.NgaySinh;
            taiXe.LoaiXe            = model.LoaiXe;
            taiXe.DiemDoi           = model.DiemDoi?.Trim();
            taiXe.TrangThaiHoatDong = model.TrangThaiHoatDong;

            if (model.DatLaiMatKhau && !string.IsNullOrWhiteSpace(model.MatKhauMoi))
                taiXe.MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhauMoi);

            try
            {
                await _context.SaveChangesAsync();
                return TaiXeServiceResult.Ok("Đã cập nhật thông tin tài xế.");
            }
            catch (Exception ex)
            {
                return TaiXeServiceResult.Fail("Lỗi khi lưu dữ liệu: " + ex.Message);
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        // Xóa
        // ──────────────────────────────────────────────────────────────────────
        public async Task<TaiXeServiceResult> XoaTaiXeAsync(string idTX)
        {
            var taiXe = await _context.TaiXes
                .Include(t => t.ChuyenDis)
                .FirstOrDefaultAsync(t => t.IdTX == idTX);
            if (taiXe == null) return TaiXeServiceResult.Miss();

            // Không xóa nếu còn chuyến đang chạy
            var dangHoatDong = taiXe.ChuyenDis?.Any(c =>
                c.TrangThai != PJGoFast.Models.Enums.TrangThaiChuyen.HOAN_TAT &&
                c.TrangThai != PJGoFast.Models.Enums.TrangThaiChuyen.HUY) ?? false;

            if (dangHoatDong)
                return TaiXeServiceResult.Fail("Không thể xóa tài xế đang có chuyến chưa hoàn tất.");

            try
            {
                _context.TaiXes.Remove(taiXe);
                await _context.SaveChangesAsync();
                return TaiXeServiceResult.Ok();
            }
            catch (Exception ex)
            {
                return TaiXeServiceResult.Fail("Lỗi khi xóa: " + ex.Message);
            }
        }
    }
}
