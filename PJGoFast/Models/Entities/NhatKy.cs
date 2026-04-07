using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PJGoFast.Models.Entities
{
    public class NhatKy
    {
        [Key]
        public string IdNhatKy { get; set; }

        [Required]
        public string IdChuyenDi { get; set; }

        public string TrangThaiCu { get; set; }
        public string TrangThaiMoi { get; set; }

        // ghi "KH"/"TX"/"Admin" dể biết ai thuc hien
        [Required]
        public string ThucHienBoi { get; set; }

        //TimeStamp
        public DateTime ThoiGian { get; set; } = DateTime.Now;

        public string LogText { get; set; }

        //Navigation properties
        [ForeignKey("IdChuyenDi")]
        public ChuyenDi ChuyenDi { get; set; }

    }
}
