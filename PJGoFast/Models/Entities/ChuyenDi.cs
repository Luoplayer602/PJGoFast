using PJGoFast.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PJGoFast.Models.Entities
{
    public class ChuyenDi
    {
        [Key]
        public string IdChuyenDi { get; set; }

        [Required]
        public string IdKH { get; set; }

        //Nullable vì có thể chưa được phân công tài xế
        public string? IdTX { get; set; }
        public string? IdAdmin { get; set; }

        [Required]
        public string DiemDon { get; set; }

        [Required]
        public string DiemDen { get; set; }

        public LoaiXe LoaiXeYeuCau { get; set; }

        public DateTime ThoiGianTao { get; set; } = DateTime.Now;

        [Required]
        public DateTime ThoiGianDon { get; set; }

        public string? GhiChu { get; set; }

        [Required]
        public TrangThaiChuyen TrangThai { get; set; } = TrangThaiChuyen.MOI;

        public decimal? GiaTamTinh { get; set; }

        public decimal? GiaThucTe { get; set; }

        //Navigation properties
        public KhachHang KhachHang { get; set; }
        public TaiXe TaiXe { get; set; }
        public Admin Admin { get; set; }
        public ThanhToan ThanhToan { get; set; }
        public ICollection<NhatKy> NhatKys { get; set; }
    }
}
