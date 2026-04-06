using System.ComponentModel.DataAnnotations;

namespace PJGoFast.Models.Entities
{
    public class Admin
    {
        [Key]
        public string IdAdmin { get; set; }

        [Required]
        public string MatKhau { get; set; } // bcrypt hash / OTP flow

        [Required]
        public string HoVaTen { get; set; }

        public string SDT { get; set; }

        public DateTime? NgaySinh { get; set; }

        // Vai trò của admin, mặc định là "DieuPhoi"
        public string VaiTro { get; set; } = "DieuPhoi";

        //Navigation properties
        public ICollection<ChuyenDi> ChuyenDis { get; set; }
    }
}
