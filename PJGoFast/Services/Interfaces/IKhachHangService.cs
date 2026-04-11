using PJGoFast.ViewModels;
using System.Security.Claims;

namespace PJGoFast.Services.Interfaces
{
    public interface IKhachHangService
    {
        int DangKy(string sdt, string matKhau, string confirmMatKhau);

        ClaimsPrincipal KiemTraDangNhap(string sdt, string matKhau);

        Models.Entities.KhachHang LayThongTinKhachHang(string idKH);

        KhachHangAccountPageVM? LayTrangTaiKhoan(string idKH);

        (bool Success, string Message) CapNhatThongTin(string idKH, KhachHangProfileUpdateVM model);

        (bool Success, string Message) DoiMatKhau(string idKH, KhachHangChangePasswordVM model);

        Task<(bool Success, string Message)> XoaTaiKhoanAsync(string idKH, string matKhauXacNhan);
    }
}
