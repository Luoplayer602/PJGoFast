using PJGoFast.Models.Entities;
using PJGoFast.Models.Enums;
using PJGoFast.ViewModels;

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

        bool XacNhanBaoGia(string id, string idKH);

        TaiXeDashboardVM? LayBangDieuKhienTaiXe(string idTX);

        TaiXeAccountVM? LayThongTinTaiKhoanTaiXe(string idTX);

        TripSummaryVM? LayChiTietChuyen(string idChuyenDi);

        DieuPhoiDashboardVM? LayBangDieuPhoi(string idAdmin);

        KhachHangTripTrackingVM LayChuyenDangTheoDoiChoKhach(string idKH);

        List<TripLogItemVM> LayNhatKyChuyen(string idChuyenDi, string? idNguoiDung = null, string? vaiTro = null);

        (bool Success, string Message) PhanCongChuyen(string idChuyenDi, string idTX, string idAdmin);

        (bool Success, string Message, string? RedirectTripId) NhanChuyen(string idChuyenDi, string idTX);

        (bool Success, string Message) TuChoiChuyenDuocPhanCong(string idChuyenDi, string idTX, bool tuDong = false);

        (bool Success, string Message) CapNhatTienTrinhTaiXe(string idChuyenDi, string idTX, TrangThaiChuyen trangThaiMoi);

        (bool Success, string Message) HuyChuyenTuTaiXe(string idChuyenDi, string idTX);

        (bool Success, string Message) XacNhanThanhToan(string idChuyenDi, string idTX, string phuongThucThanhToan, decimal soTienThanhToan);
    }
}
