// ============================================================
// Thêm vào ViewModel.cs — phần TaiXe CRUD ViewModels
// Thêm vào namespace PJGoFast.ViewModels, bên dưới ThanhToanVM
// ============================================================

using PJGoFast.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PJGoFast.ViewModels
{
    // ── Index (danh sách) ────────────────────────────────────────────────────
    public class TaiXeManagementIndexVM
    {
        public List<TaiXeRowVM> TaiXes { get; set; } = new();

        // Tổng hợp cho summary bar
        public int TongSo           => TaiXes.Count;
        public int SoOnline         => TaiXes.Count(t => t.TrangThaiOnline == "ONLINE");
        public int SoBusy           => TaiXes.Count(t => t.TrangThaiOnline == "BUSY");
        public int SoOffline        => TaiXes.Count(t => t.TrangThaiOnline == "OFFLINE");
        public int SoHoatDong       => TaiXes.Count(t => t.TrangThaiHoatDong == "HoatDong");
        public int SoNghiViec       => TaiXes.Count(t => t.TrangThaiHoatDong != "HoatDong");
    }

    public class TaiXeRowVM
    {
        public string IdTX              { get; set; }
        public string HoVaTen           { get; set; }
        public string SDT               { get; set; }
        public DateTime? NgaySinh       { get; set; }
        public string TrangThaiHoatDong { get; set; }
        public string TrangThaiOnline   { get; set; }
        public string LoaiXe            { get; set; }   // Display Name
        public string? DiemDoi          { get; set; }
    }

    // ── Details ──────────────────────────────────────────────────────────────
    public class TaiXeDetailsVM
    {
        public string IdTX              { get; set; }
        public string HoVaTen           { get; set; }
        public string SDT               { get; set; }
        public DateTime? NgaySinh       { get; set; }
        public string TrangThaiHoatDong { get; set; }
        public string TrangThaiOnline   { get; set; }
        public LoaiXe LoaiXe            { get; set; }
        public string? DiemDoi          { get; set; }
        public string? ViTri            { get; set; }
        public int SoChuyenDaLam        { get; set; }  // thống kê nhanh
    }

    // ── Create ───────────────────────────────────────────────────────────────
    public class TaiXeCreateVM
    {
        [Required(ErrorMessage = "Mã tài xế không được để trống")]
        [Display(Name = "Mã tài xế")]
        [RegularExpression(@"^TX\d+$", ErrorMessage = "Mã tài xế phải bắt đầu bằng TX và theo sau là số (VD: TX001)")]
        public string IdTX { get; set; }

        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [Display(Name = "Họ và tên")]
        public string HoVaTen { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Display(Name = "Số điện thoại")]
        public string SDT { get; set; }

        [Display(Name = "Ngày sinh")]
        public DateTime? NgaySinh { get; set; }

        [Required(ErrorMessage = "Loại xe không được để trống")]
        [Display(Name = "Loại xe")]
        public LoaiXe LoaiXe { get; set; }

        [Display(Name = "Điểm đỗ")]
        public string? DiemDoi { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
        [Compare(nameof(MatKhau), ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [Display(Name = "Xác nhận mật khẩu")]
        public string XacNhanMatKhau { get; set; }
    }

    // ── Edit ─────────────────────────────────────────────────────────────────
    public class TaiXeEditVM
    {
        public string IdTX { get; set; }    // readonly, không cho sửa

        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [Display(Name = "Họ và tên")]
        public string HoVaTen { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Display(Name = "Số điện thoại")]
        public string SDT { get; set; }

        [Display(Name = "Ngày sinh")]
        public DateTime? NgaySinh { get; set; }

        [Display(Name = "Loại xe")]
        public LoaiXe LoaiXe { get; set; }

        [Display(Name = "Điểm đỗ")]
        public string? DiemDoi { get; set; }

        [Display(Name = "Trạng thái hoạt động")]
        public string TrangThaiHoatDong { get; set; } = "HoatDong";

        // Đặt lại mật khẩu (tuỳ chọn)
        public bool DatLaiMatKhau { get; set; } = false;

        [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        [Display(Name = "Mật khẩu mới")]
        public string? MatKhauMoi { get; set; }

        [Compare(nameof(MatKhauMoi), ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [Display(Name = "Xác nhận mật khẩu mới")]
        public string? XacNhanMatKhauMoi { get; set; }
    }

    // ── Kết quả thao tác (dùng chung với Admin pattern) ──────────────────────
    public class TaiXeServiceResult
    {
        public bool Success      { get; set; }
        public bool NotFound     { get; set; }
        public string? ErrorMessage { get; set; }

        public static TaiXeServiceResult Ok(string? msg = null)
            => new() { Success = true, ErrorMessage = msg };
        public static TaiXeServiceResult Fail(string msg)
            => new() { Success = false, ErrorMessage = msg };
        public static TaiXeServiceResult Miss()
            => new() { NotFound = true };
    }
}
