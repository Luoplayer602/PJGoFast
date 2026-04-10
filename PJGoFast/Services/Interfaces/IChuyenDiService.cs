using PJGoFast.Models.Entities;
using PJGoFast.Models.Enums;

namespace PJGoFast.Services.Interfaces
{
    public interface IChuyenDiService
    {
        string TaoChuyenDi(string diemDon, string diemDen, DateTime TGDon, LoaiXe loaiXe, string ghiChu, string IdKhachHang);

        ChuyenDi? LayChuyenDiTheoId(string id);

        void CapNhatGiaTamTinh(string id, decimal gia);

        List<ChuyenDi> LayDanhSachChuyenDiCuaKH(string idKH);

        bool HuyChuyenDi(string id, string idKH);

        bool XoaChuyenDi(string id, string idKH);
    }
}
