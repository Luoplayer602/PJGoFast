using PJGoFast.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

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
        public string IdKH { get; set; }
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
        [Display(Name = "Mã chuyến đi")]
        public string IdChuyenDi { get; set; }
        [Display(Name = "Mã khách hàng")]
        public string IdKH { get; set; }
        [Display(Name = "Điểm đón")]
        public string DiemDon { get; set; }
        [Display(Name = "Điểm đến")]
        public string DiemDen { get; set; }
        [Display(Name = "Trạng thái")]
        public TrangThaiChuyen TrangThai { get; set; }
        [Display(Name = "Giá tạm tính")]
        public decimal GiaTamTinh { get; set; }
        [Display(Name = "Giá thực tế")]
        public decimal? GiaThucTe { get; set; }
        [Display(Name = "Thời gian tạo yêu cầu")]
        [DisplayFormat(DataFormatString = "{0:HH:mm dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ThoiGianTao { get; set; }
        [Display(Name = "Thời gian đón")]
        [DisplayFormat(DataFormatString = "{0:HH:mm dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ThoiGianDon { get; set; }
        [Display(Name = "Ghi chú")]
        public string GhiChu { get; set; }
        [Display(Name = "Tên khách hàng")]
        public string TenKhachHang { get; set; }
        [Display(Name = "SDT khách hàng")]
        public string SDTKhachHang { get; set; }
        [Display(Name = "Tên tài xế nhận chuyến")]
        public string? TenTaiXe { get; set; }
        [Display(Name = "SDT tài xế nhận chuyến")]
        public string? SDTTaiXe { get; set; }
        [Display(Name = "Mã tài xế nhận chuyến")]
        public string? IdTX { get; set; }
        [Display(Name = "Tên admin phân công")]
        public string? TenAdmin { get; set; }
        [Display(Name = "SDT admin phân công")]
        public string? SDTAdmin { get; set; }
        [Display(Name = "Mã admin phân công")]
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
        public LoaiXe LoaiXeYeuCau { get; set; }

        [Required(ErrorMessage = "Thời gian đón không được để trống")]
        [DisplayFormat(DataFormatString = "{0:HH:mm ddMMyyyy}", ApplyFormatInEditMode = true)]
        public DateTime ThoiGianDon { get; set; } = DateTime.Now;

        public DateTime ThoiGianTao { get; set; } = DateTime.Now;

        [AllowNull]
        public string? GhiChu { get; set; }
    }

    /// <summary>ViewModel cho màn hình báo giá sau khi tạo chuyến đi thành công.</summary>
    public class BaoGiaVM
    {
        // ── Thông tin chuyến đi ──────────────────────────────────────────────
        public string IdChuyenDi { get; set; }
        public string DiemDon { get; set; }
        public string DiemDen { get; set; }
        public DateTime ThoiGianDon { get; set; }
        public LoaiXe LoaiXeYeuCau { get; set; }
        public string? GhiChu { get; set; }

        // ── Kết quả từ Goong Distance Matrix ─────────────────────────────────
        /// <summary>Khoảng cách (km), làm tròn 1 chữ số thập phân.</summary>
        public double KhoangCachKm { get; set; }
        /// <summary>Thời gian di chuyển ước tính (phút).</summary>
        public int ThoiGianDuTinhPhut { get; set; }

        // ── Báo giá ───────────────────────────────────────────────────────────
        /// <summary>Giá tạm tính (VND).</summary>
        public decimal GiaTamTinh { get; set; }
        /// <summary>Đơn giá theo loại xe (VND/km).</summary>
        public decimal GiaPerKm { get; set; }
        /// <summary>True nếu áp dụng giá tối thiểu thay vì giá theo km.</summary>
        public bool ApDungGiaToiThieu { get; set; }

        // ── Tọa độ để hiển thị bản đồ ────────────────────────────────────────
        public double DonLat { get; set; }
        public double DonLng { get; set; }
        public double DenLat { get; set; }
        public double DenLng { get; set; }

        // ── API keys truyền xuống view bản đồ ────────────────────────────────
        public string? GoongMapKey { get; set; }
        public string? GoongApiKey { get; set; }
    }

    public class ThanhToanVM
    {
        public string IdThanhToan { get; set; }
        public string TrangThaiThanhToan { get; set; }
        public string PhuongThucThanhToan { get; set; }
        public decimal SoTienThanhToan { get; set; }
        public DateTime ThoiGianGhiNhan { get; set; }
        public string IdChuyenDi { get; set; }
    }
}
