using System.ComponentModel.DataAnnotations;

namespace PJGoFast.Models.Entities
{
    public class KhachHang
    {
        [Key]
        public string IdKH { get; set; }

        [Required]
        public string SDT { get; set; }

        [Required]
        public string HoVaTen { get; set; }

        public DateTime? NgaySinh { get; set; }

        public string? Email { get; set; }

        public DateTime NgayDangKy { get; set; }

        [Required]
        public string MatKhau { get; set; }  // bcrypt hash / OTP flow

        //Navigation properties
        public ICollection<ChuyenDi> ChuyenDis { get; set; }
    }
}
