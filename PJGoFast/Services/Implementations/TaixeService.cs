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

        public ClaimsPrincipal KiemTraDangNhap(string sdt, string matKhau)
        {
            var taiXe = _context.TaiXes.FirstOrDefault(t => t.SDT == sdt.Trim());
            if (taiXe == null || !BCrypt.Net.BCrypt.Verify(matKhau, taiXe.MatKhau))
            {
                return null;
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, taiXe.IdTX),
                new(ClaimTypes.Name, taiXe.HoVaTen),
                new(ClaimTypes.Role, "TaiXe")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(identity);
        }

        public async Task<TaiXeManagementIndexVM> LayDanhSachAsync()
        {
            var list = await _context.TaiXes
                .OrderBy(t => t.HoVaTen)
                .Select(t => new TaiXeRowVM
                {
                    IdTX = t.IdTX,
                    HoVaTen = t.HoVaTen,
                    SDT = t.SDT,
                    NgaySinh = t.NgaySinh,
                    TrangThaiHoatDong = t.TrangThaiHoatDong,
                    TrangThaiOnline = t.TrangThaiOnline.ToString(),
                    LoaiXe = t.LoaiXe.ToString(),
                    DiemDoi = t.DiemDoi
                })
                .ToListAsync();

            return new TaiXeManagementIndexVM { TaiXes = list };
        }

        public async Task<TaiXeDetailsVM?> LayChiTietAsync(string idTX)
        {
            var taiXe = await _context.TaiXes
                .Include(x => x.ChuyenDis)
                .FirstOrDefaultAsync(x => x.IdTX == idTX);

            if (taiXe == null)
            {
                return null;
            }

            return new TaiXeDetailsVM
            {
                IdTX = taiXe.IdTX,
                HoVaTen = taiXe.HoVaTen,
                SDT = taiXe.SDT,
                NgaySinh = taiXe.NgaySinh,
                TrangThaiHoatDong = taiXe.TrangThaiHoatDong,
                TrangThaiOnline = taiXe.TrangThaiOnline.ToString(),
                LoaiXe = taiXe.LoaiXe,
                DiemDoi = taiXe.DiemDoi,
                ViTri = taiXe.ViTri,
                SoChuyenDaLam = taiXe.ChuyenDis?.Count ?? 0
            };
        }

        public async Task<TaiXeEditVM?> LayTaiXeDeSuaAsync(string idTX)
        {
            var taiXe = await _context.TaiXes.FindAsync(idTX);
            if (taiXe == null)
            {
                return null;
            }

            return new TaiXeEditVM
            {
                IdTX = taiXe.IdTX,
                HoVaTen = taiXe.HoVaTen,
                SDT = taiXe.SDT,
                NgaySinh = taiXe.NgaySinh,
                LoaiXe = taiXe.LoaiXe,
                DiemDoi = taiXe.DiemDoi,
                TrangThaiHoatDong = taiXe.TrangThaiHoatDong
            };
        }

        public async Task<TaiXeServiceResult> TaoTaiXeAsync(TaiXeCreateVM model)
        {
            if (await _context.TaiXes.AnyAsync(t => t.IdTX == model.IdTX))
            {
                return TaiXeServiceResult.Fail($"Mã tài xế '{model.IdTX}' đã tồn tại.");
            }

            if (await _context.TaiXes.AnyAsync(t => t.SDT == model.SDT.Trim()))
            {
                return TaiXeServiceResult.Fail($"Số điện thoại '{model.SDT}' đã được đăng ký.");
            }

            var taiXe = new TaiXe
            {
                IdTX = model.IdTX.Trim().ToUpper(),
                HoVaTen = model.HoVaTen.Trim(),
                SDT = model.SDT.Trim(),
                NgaySinh = model.NgaySinh,
                LoaiXe = model.LoaiXe,
                DiemDoi = model.DiemDoi?.Trim(),
                TrangThaiHoatDong = "HoatDong",
                TrangThaiOnline = TrangThaiOnline.OFFLINE,
                MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau)
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

        public async Task<TaiXeServiceResult> CapNhatTaiXeAsync(TaiXeEditVM model)
        {
            var taiXe = await _context.TaiXes.FindAsync(model.IdTX);
            if (taiXe == null)
            {
                return TaiXeServiceResult.Miss();
            }

            if (await _context.TaiXes.AnyAsync(t => t.SDT == model.SDT.Trim() && t.IdTX != model.IdTX))
            {
                return TaiXeServiceResult.Fail($"Số điện thoại '{model.SDT}' đã được đăng ký bởi tài xế khác.");
            }

            taiXe.HoVaTen = model.HoVaTen.Trim();
            taiXe.SDT = model.SDT.Trim();
            taiXe.NgaySinh = model.NgaySinh;
            taiXe.LoaiXe = model.LoaiXe;
            taiXe.DiemDoi = model.DiemDoi?.Trim();
            taiXe.TrangThaiHoatDong = model.TrangThaiHoatDong;

            if (model.DatLaiMatKhau && !string.IsNullOrWhiteSpace(model.MatKhauMoi))
            {
                taiXe.MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhauMoi);
            }

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

        public async Task<TaiXeServiceResult> XoaTaiXeAsync(string idTX)
        {
            var taiXe = await _context.TaiXes
                .Include(t => t.ChuyenDis)
                .FirstOrDefaultAsync(t => t.IdTX == idTX);

            if (taiXe == null)
            {
                return TaiXeServiceResult.Miss();
            }

            var dangHoatDong = taiXe.ChuyenDis?.Any(c =>
                c.TrangThai != TrangThaiChuyen.HOAN_TAT &&
                c.TrangThai != TrangThaiChuyen.HUY) ?? false;

            if (dangHoatDong)
            {
                return TaiXeServiceResult.Fail("Không thể xóa tài xế đang có chuyến chưa hoàn tất.");
            }

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

        public async Task<(bool Success, string Message, TrangThaiOnline TrangThai)> CapNhatTrangThaiOnlineAsync(string idTX, TrangThaiOnline mucTieu)
        {
            var taiXe = await _context.TaiXes
                .Include(t => t.ChuyenDis)
                .ThenInclude(c => c.ThanhToan)
                .FirstOrDefaultAsync(t => t.IdTX == idTX);

            if (taiXe == null)
            {
                return (false, "Không tìm thấy tài xế.", TrangThaiOnline.OFFLINE);
            }

            var dangBan = taiXe.ChuyenDis.Any(c =>
                c.TrangThai == TrangThaiChuyen.DA_NHAN ||
                c.TrangThai == TrangThaiChuyen.DANG_DON ||
                c.TrangThai == TrangThaiChuyen.DA_DON ||
                c.TrangThai == TrangThaiChuyen.DANG_DI_CHUYEN ||
                (c.TrangThai == TrangThaiChuyen.HOAN_TAT && c.ThanhToan == null));

            taiXe.TrangThaiOnline = dangBan ? TrangThaiOnline.BUSY : mucTieu;
            await _context.SaveChangesAsync();

            var message = dangBan
                ? "Tài xế đang có chuyến hoạt động nên giữ trạng thái BUSY."
                : "Đã cập nhật trạng thái hoạt động.";

            return (true, message, taiXe.TrangThaiOnline);
        }
    }
}
