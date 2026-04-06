using System.ComponentModel.DataAnnotations;
using PJGoFast.Models.Enums;

namespace PJGoFast.Models.Entities
{
    public class TaiXe
    {
        [Key]
        public string IdTX { get; set; }

        [Required]
        public string SDT { get; set; }

        [Required]
        public string HoVaTen { get; set; }

        public DateTime? NgaySinh { get; set; }

        //TrangThaiHoatDong
        public string TrangThaiHoatDong { get; set; } = "HoatDong";

        public LoaiXe LoaiXe { get; set; }

        public string DiemDoi { get; set; }

        public string ViTri { get; set; }

        //TrangThaiOnline
        public TrangThaiOnline TrangThaiOnline { get; set; } = TrangThaiOnline.OFFLINE;

        [Required]
        public string MatKhau { get; set; }  // bcrypt hash / OTP flow

        //Navigation properties
        public ICollection<ChuyenDi> ChuyenDis { get; set; }
    }
}
