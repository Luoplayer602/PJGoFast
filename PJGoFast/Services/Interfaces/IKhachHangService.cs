using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace PJGoFast.Services.Interfaces
{
    public interface IKhachHangService
    {
        int DangKy(string sdt, string matKhau, string confirmMatKhau);

        ClaimsPrincipal KiemTraDangNhap(string sdt, string matKhau);
    }
}
