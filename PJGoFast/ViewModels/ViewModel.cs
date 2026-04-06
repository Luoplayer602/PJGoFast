using System.ComponentModel.DataAnnotations;

namespace PJGoFast.ViewModels
{
    public class AdminVM
    {
        public string IdAdmin { get; set; }

        public string HoVaTen { get; set; }

        public string SDT { get; set; }

        public DateTime? NgaySinh { get; set; }

        public string VaiTro { get; set; }
    }

    public class KhachHangVM
    {
        public string IdKH{ get; set; }
        public string HoVaTen { get; set; }
        public string SDT { get; set; }
        public string? Email { get; set; }
        public DateTime? NgaySinh { get; set; }
        public DateTime NgayDangKy { get; set; }
    }

    public class TaiXeVM
    {
        public string IdTX { get; set; }

        public string HoVaTen { get; set; }

        public string SDT { get; set; }

        public string TrangThaiHoatDong { get; set; }

        public string? DiemDoi { get; set; }

        public string? ViTri { get; set; }

        public string TrangThaiOnline { get; set; }

        public string? LoaiXe { get; set; }
    }

    public class NhatKyVM
    {
        public string IdNhatKy { get; set; }

        public string TrangThaiCu { get; set; }

        public string TrangThaiMoi { get; set; }

        public string ThucHienBoi { get; set; }

        public DateTime ThoiGian { get; set; }

        public string LogText { get; set; }
    }

    public class ChuyenDiVM
    {
        public string IdChuyenDi { get; set; }

        public string IdKH { get; set; }

        public string DiemDon { get; set; }

        public string DiemDen { get; set; }

        public string TrangThai { get; set; }

        public decimal GiaTamTinh { get; set; }

        public decimal? GiaThucTe { get; set; }

        public DateTime ThoiGianTao { get; set; }
        public DateTime ThoiGianDon { get; set; }

        public string GhiChu { get; set; }

        // ===== Related Data =====
        public string TenKhachHang { get; set; }
        public string SDTKhachHang { get; set; }

        public string? TenTaiXe { get; set; }
        public string? SDTTaiXe { get; set; }
        public string? IdTX { get; set; }

        public string? TenAdmin { get; set; }
        public string? SDTAdmin { get; set; }
        public string? IdAdmin { get; set; }
    }

    public class ChuyenDiCreateVM
    {
        [Required]
        public string IdKH { get; set; }

        [Required(ErrorMessage = "Điểm đón không được để trống")] 
        public string DiemDon { get; set; }
        [Required(ErrorMessage = "Điểm đến không được để trống")]

        public string DiemDen { get; set; }
        [Required(ErrorMessage = "Loại xe yêu cầu không được để trống")]

        public string LoaiXeYeuCau { get; set; }
        [Required(ErrorMessage = "Thời gian đón không được để trống")]
        public DateTime ThoiGianDon { get; set; }
        public DateTime ThoiGianTao { get; set; } = DateTime.Now;

        public string GhiChu { get; set; }
    }

    public class ThanhToanVM
    {
        public string IdThanhToan { get; set; }

        public string TrangThaiThanhToan { get; set; }

        public string PhuongThucThanhToan { get; set; }

        public decimal SoTienThanhToan { get; set; }

        public DateTime ThoiGianGhiNhan { get; set; }

        // Thông tin liên kết
        public string IdChuyenDi { get; set; }
    }


}
