using System.ComponentModel.DataAnnotations;

namespace PJGoFast.ViewModels
{
    public static class AdminRoleOptions
    {
        public const string QuanTri = "QuanTri";
        public const string DieuPhoi = "DieuPhoi";

        public static readonly string[] All = [QuanTri, DieuPhoi];
    }

    public class AdminListItemVM
    {
        public string IdAdmin { get; set; } = string.Empty;
        public string HoVaTen { get; set; } = string.Empty;
        public string SDT { get; set; } = string.Empty;
        public DateTime? NgaySinh { get; set; }
        public string VaiTro { get; set; } = string.Empty;
    }

    public class AdminManagementIndexVM
    {
        public List<AdminListItemVM> Admins { get; set; } = [];
        public int TongSoAdmin => Admins.Count;
        public int SoQuanTri => Admins.Count(a => a.VaiTro == AdminRoleOptions.QuanTri);
        public int SoDieuPhoi => Admins.Count(a => a.VaiTro == AdminRoleOptions.DieuPhoi);
    }

    public class AdminDetailsVM
    {
        public string IdAdmin { get; set; } = string.Empty;
        public string HoVaTen { get; set; } = string.Empty;
        public string SDT { get; set; } = string.Empty;
        public DateTime? NgaySinh { get; set; }
        public string VaiTro { get; set; } = string.Empty;
    }

    public class AdminCreateVM : IValidatableObject
    {
        [Required(ErrorMessage = "Mã admin không được để trống.")]
        [Display(Name = "Mã admin")]
        public string IdAdmin { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ và tên không được để trống.")]
        [Display(Name = "Họ và tên")]
        public string HoVaTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [Display(Name = "Số điện thoại")]
        public string SDT { get; set; } = string.Empty;

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [Required(ErrorMessage = "Vai trò không được để trống.")]
        [Display(Name = "Vai trò")]
        public string VaiTro { get; set; } = AdminRoleOptions.DieuPhoi;

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống.")]
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        public string XacNhanMatKhau { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!AdminRoleOptions.All.Contains(VaiTro))
            {
                yield return new ValidationResult("Vai trò không hợp lệ.", [nameof(VaiTro)]);
            }

            if (!string.Equals(MatKhau, XacNhanMatKhau, StringComparison.Ordinal))
            {
                yield return new ValidationResult("Mật khẩu xác nhận không khớp.", [nameof(XacNhanMatKhau)]);
            }

            if (!string.IsNullOrWhiteSpace(MatKhau) && MatKhau.Trim().Length < 6)
            {
                yield return new ValidationResult("Mật khẩu phải có ít nhất 6 ký tự.", [nameof(MatKhau)]);
            }
        }
    }

    public class AdminEditVM : IValidatableObject
    {
        [Required]
        public string IdAdmin { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ và tên không được để trống.")]
        [Display(Name = "Họ và tên")]
        public string HoVaTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [Display(Name = "Số điện thoại")]
        public string SDT { get; set; } = string.Empty;

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [Required(ErrorMessage = "Vai trò không được để trống.")]
        [Display(Name = "Vai trò")]
        public string VaiTro { get; set; } = AdminRoleOptions.DieuPhoi;

        [Display(Name = "Đặt lại mật khẩu")]
        public bool DatLaiMatKhau { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string? MatKhauMoi { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu mới")]
        public string? XacNhanMatKhauMoi { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!AdminRoleOptions.All.Contains(VaiTro))
            {
                yield return new ValidationResult("Vai trò không hợp lệ.", [nameof(VaiTro)]);
            }

            if (!DatLaiMatKhau)
            {
                yield break;
            }

            if (string.IsNullOrWhiteSpace(MatKhauMoi))
            {
                yield return new ValidationResult("Vui lòng nhập mật khẩu mới.", [nameof(MatKhauMoi)]);
            }

            if (string.IsNullOrWhiteSpace(XacNhanMatKhauMoi))
            {
                yield return new ValidationResult("Vui lòng xác nhận mật khẩu mới.", [nameof(XacNhanMatKhauMoi)]);
            }

            if (!string.IsNullOrWhiteSpace(MatKhauMoi) && MatKhauMoi.Trim().Length < 6)
            {
                yield return new ValidationResult("Mật khẩu mới phải có ít nhất 6 ký tự.", [nameof(MatKhauMoi)]);
            }

            if (!string.IsNullOrWhiteSpace(MatKhauMoi) &&
                !string.Equals(MatKhauMoi, XacNhanMatKhauMoi, StringComparison.Ordinal))
            {
                yield return new ValidationResult("Mật khẩu xác nhận không khớp.", [nameof(XacNhanMatKhauMoi)]);
            }
        }
    }
}
