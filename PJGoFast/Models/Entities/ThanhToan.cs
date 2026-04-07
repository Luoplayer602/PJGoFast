using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PJGoFast.Models.Entities
{
    public class ThanhToan
    {
        [Key]
        public string IdThanhToan { get; set; }

        [Required]
        public string IdChuyenDi { get; set; }

        public string TrangThaiThanhToan { get; set; } = "ChuaThanhToan";

        public string PhuongThucThanhToan { get; set; } = "TienMat"; // Mặc định là tiền mặt, có thể mở rộng thêm phương thức khác

        public decimal SoTienThanhToan { get; set; }

        public DateTime ThoiGianGhiNhan { get; set; } = DateTime.Now;

        //Navigation properties
        [ForeignKey("IdChuyenDi")]
        public ChuyenDi ChuyenDi { get; set; }
    }

    
      
    
}