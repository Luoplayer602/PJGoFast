using System.ComponentModel.DataAnnotations;

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
        public ChuyenDi ChuyenDi { get; set; }
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