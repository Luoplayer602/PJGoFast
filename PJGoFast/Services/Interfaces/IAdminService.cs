using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PJGoFast.Services.Interfaces
{
    public interface IAdminService
    {
        ClaimsPrincipal KiemTraDangNhap(string sdt, string matKhau);
    }
}
