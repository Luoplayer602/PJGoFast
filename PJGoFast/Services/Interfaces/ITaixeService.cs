// ============================================================
// ITaiXeService.cs  — thêm vào Services/Interfaces/
// Giữ nguyên method KiemTraDangNhap cũ, bổ sung CRUD admin
// ============================================================

using PJGoFast.ViewModels;
using System.Security.Claims;

namespace PJGoFast.Services.Interfaces
{
    public interface ITaiXeService
    {
        // ── Đăng nhập tài xế (đã có) ─────────────────────────────────────────
        ClaimsPrincipal KiemTraDangNhap(string sdt, string matKhau);

        // ── CRUD dành cho Admin ───────────────────────────────────────────────
        Task<TaiXeManagementIndexVM> LayDanhSachAsync();
        Task<TaiXeDetailsVM?> LayChiTietAsync(string idTX);
        Task<TaiXeEditVM?>    LayTaiXeDeSuaAsync(string idTX);
        Task<TaiXeServiceResult> TaoTaiXeAsync(TaiXeCreateVM model);
        Task<TaiXeServiceResult> CapNhatTaiXeAsync(TaiXeEditVM model);
        Task<TaiXeServiceResult> XoaTaiXeAsync(string idTX);
    }
}
