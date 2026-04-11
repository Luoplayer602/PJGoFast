using System.ComponentModel.DataAnnotations;
using PJGoFast.Models.Enums;

namespace PJGoFast.ViewModels
{
    public class TripParticipantVM
    {
        public string? Id { get; set; }
        public string? HoVaTen { get; set; }
        public string? SDT { get; set; }
        public string? VaiTro { get; set; }
        public string? ViTri { get; set; }
        public string? DiemDoi { get; set; }
        public string? LoaiXe { get; set; }
        public string? TrangThaiOnline { get; set; }
    }

    public class TripLogItemVM
    {
        public string IdNhatKy { get; set; } = string.Empty;
        public string TrangThaiCu { get; set; } = string.Empty;
        public string TrangThaiMoi { get; set; } = string.Empty;
        public string ThucHienBoi { get; set; } = string.Empty;
        public DateTime ThoiGian { get; set; }
        public string LogText { get; set; } = string.Empty;
    }

    public class TripSummaryVM
    {
        public string IdChuyenDi { get; set; } = string.Empty;
        public string IdKH { get; set; } = string.Empty;
        public string? IdTX { get; set; }
        public string? IdAdmin { get; set; }
        public string DiemDon { get; set; } = string.Empty;
        public string DiemDen { get; set; } = string.Empty;
        public LoaiXe LoaiXeYeuCau { get; set; }
        public TrangThaiChuyen TrangThai { get; set; }
        public DateTime ThoiGianTao { get; set; }
        public DateTime ThoiGianDon { get; set; }
        public DateTime? HanNhanChuyen { get; set; }
        public string? GhiChu { get; set; }
        public decimal? GiaTamTinh { get; set; }
        public decimal? GiaThucTe { get; set; }
        public int? RemainingSeconds { get; set; }
        public bool IsAssignedToCurrentDriver { get; set; }
        public bool CanReceive { get; set; }
        public bool CanReject { get; set; }
        public bool CanCancel { get; set; }
        public bool HasPayment { get; set; }
        public string? PhuongThucThanhToan { get; set; }
        public decimal? SoTienThanhToan { get; set; }
        public TripParticipantVM? KhachHang { get; set; }
        public TripParticipantVM? TaiXe { get; set; }
        public TripParticipantVM? Admin { get; set; }
        public List<TripLogItemVM> Logs { get; set; } = [];
    }

    public class TaiXeDashboardVM
    {
        public string IdTX { get; set; } = string.Empty;
        public string HoVaTen { get; set; } = string.Empty;
        public string SDT { get; set; } = string.Empty;
        public string? ViTri { get; set; }
        public string? DiemDoi { get; set; }
        public string LoaiXe { get; set; } = string.Empty;
        public TrangThaiOnline TrangThaiOnline { get; set; }
        public bool DangBan { get; set; }
        public TripSummaryVM? ChuyenDangThucHien { get; set; }
        public List<TripSummaryVM> ChuyenDuocPhanCong { get; set; } = [];
        public List<TripSummaryVM> ChuyenMoi { get; set; } = [];
        public List<TripLogItemVM> ThongBaoMoi { get; set; } = [];
    }

    public class TaiXeAccountVM
    {
        public string IdTX { get; set; } = string.Empty;
        public string HoVaTen { get; set; } = string.Empty;
        public string SDT { get; set; } = string.Empty;
        public DateTime? NgaySinh { get; set; }
        public string TrangThaiHoatDong { get; set; } = string.Empty;
        public TrangThaiOnline TrangThaiOnline { get; set; }
        public LoaiXe LoaiXe { get; set; }
        public string? DiemDoi { get; set; }
        public string? ViTri { get; set; }
        public int TongChuyen { get; set; }
        public int ChuyenHoanTat { get; set; }
        public int ChuyenDangXuLy { get; set; }
    }

    public class DieuPhoiDriverVM
    {
        public string IdTX { get; set; } = string.Empty;
        public string HoVaTen { get; set; } = string.Empty;
        public string SDT { get; set; } = string.Empty;
        public string? ViTri { get; set; }
        public string? DiemDoi { get; set; }
        public string LoaiXe { get; set; } = string.Empty;
        public TrangThaiOnline TrangThaiOnline { get; set; }
        public bool DangBan { get; set; }
        public TripSummaryVM? ChuyenDangXuLy { get; set; }
    }

    public class DieuPhoiDashboardVM
    {
        public string IdAdmin { get; set; } = string.Empty;
        public string HoVaTen { get; set; } = string.Empty;
        public List<TripSummaryVM> ChuyenMoi { get; set; } = [];
        public List<DieuPhoiDriverVM> TaiXes { get; set; } = [];
        public List<TripLogItemVM> SuKienMoi { get; set; } = [];
    }

    public class KhachHangTripTrackingVM
    {
        public TripSummaryVM? ChuyenDangTheoDoi { get; set; }
        public List<TripSummaryVM> ChuyenDangXuLy { get; set; } = [];
        public List<TripLogItemVM> Logs { get; set; } = [];
    }

    public class KhachHangAccountPageVM
    {
        public Models.Entities.KhachHang KhachHang { get; set; } = null!;
        public KhachHangProfileUpdateVM ProfileForm { get; set; } = new();
        public KhachHangChangePasswordVM PasswordForm { get; set; } = new();
        public KhachHangDeleteAccountVM DeleteForm { get; set; } = new();
    }

    public class KhachHangProfileUpdateVM
    {
        [Required(ErrorMessage = "Họ và tên không được để trống.")]
        [Display(Name = "Họ và tên")]
        public string HoVaTen { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string? Email { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }
    }

    public class KhachHangChangePasswordVM
    {
        [Required(ErrorMessage = "Mật khẩu hiện tại không được để trống.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu hiện tại")]
        public string MatKhauHienTai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu mới không được để trống.")]
        [MinLength(6, ErrorMessage = "Mật khẩu mới tối thiểu 6 ký tự.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string MatKhauMoi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Xác nhận mật khẩu mới không được để trống.")]
        [Compare(nameof(MatKhauMoi), ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu mới")]
        public string XacNhanMatKhauMoi { get; set; } = string.Empty;
    }

    public class KhachHangDeleteAccountVM
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu để xác nhận.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu xác nhận")]
        public string MatKhauXacNhan { get; set; } = string.Empty;
    }

    public class XacNhanThanhToanVM
    {
        [Required]
        public string IdChuyenDi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phương thức thanh toán không được để trống.")]
        [Display(Name = "Phương thức thanh toán")]
        public string PhuongThucThanhToan { get; set; } = "TienMat";

        [Range(0, double.MaxValue, ErrorMessage = "Số tiền thu hộ không hợp lệ.")]
        [Display(Name = "Số tiền thu hộ")]
        public decimal SoTienThanhToan { get; set; }
    }
}
