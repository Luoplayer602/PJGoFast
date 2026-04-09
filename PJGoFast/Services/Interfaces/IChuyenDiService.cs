using PJGoFast.Models.Enums;

namespace PJGoFast.Services.Interfaces
{
    public interface IChuyenDiService
    {
        string TaoChuyenDi(string diemDon, string diemDen, DateTime TGDon, LoaiXe loaiXe, string ghiChu, string IdKhachHang);
    }
}
