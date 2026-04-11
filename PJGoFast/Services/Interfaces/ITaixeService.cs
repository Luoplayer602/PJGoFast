using PJGoFast.Models.Enums;
using PJGoFast.ViewModels;
using System.Security.Claims;

namespace PJGoFast.Services.Interfaces
{
    public interface ITaiXeService
    {
        ClaimsPrincipal KiemTraDangNhap(string sdt, string matKhau);

        Task<TaiXeManagementIndexVM> LayDanhSachAsync();
        Task<TaiXeDetailsVM?> LayChiTietAsync(string idTX);
        Task<TaiXeEditVM?> LayTaiXeDeSuaAsync(string idTX);
        Task<TaiXeServiceResult> TaoTaiXeAsync(TaiXeCreateVM model);
        Task<TaiXeServiceResult> CapNhatTaiXeAsync(TaiXeEditVM model);
        Task<TaiXeServiceResult> XoaTaiXeAsync(string idTX);
        Task<(bool Success, string Message, TrangThaiOnline TrangThai)> CapNhatTrangThaiOnlineAsync(string idTX, TrangThaiOnline mucTieu);
    }
}
