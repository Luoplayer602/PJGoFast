using PJGoFast.ViewModels;
using System.Security.Claims;

namespace PJGoFast.Services.Interfaces
{
    public interface IAdminService
    {
        ClaimsPrincipal? KiemTraDangNhap(string sdt, string matKhau);
        Task<AdminManagementIndexVM> LayDanhSachQuanLyAsync();
        Task<AdminDetailsVM?> LayChiTietAsync(string idAdmin);
        Task<AdminEditVM?> LayAdminDeSuaAsync(string idAdmin);
        Task<(bool Success, string? ErrorMessage)> TaoAdminAsync(AdminCreateVM model);
        Task<(bool Success, bool NotFound, string? ErrorMessage)> CapNhatAdminAsync(AdminEditVM model, string? currentAdminId);
        Task<(bool Success, bool NotFound, string? ErrorMessage)> XoaAdminAsync(string idAdmin, string? currentAdminId);
    }
}
