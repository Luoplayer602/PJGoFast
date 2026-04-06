using PJGoFast.Models.Entities;
using PJGoFast.Data;
using PJGoFast.Services.Interfaces;
using PJGoFast.Models.Enums;
using PJGoFast.ViewModels;


namespace PJGoFast.Services.Implementations
{
    public class ChuyenDiService : IChuyenDiService
    {
        private readonly PJGoFastDbContext _context;

        private readonly ILogger<ChuyenDiService> _logger;

        public ChuyenDiService(PJGoFastDbContext context)
        {
            _context = context;
            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<ChuyenDiService>();
        }

        public int TaoChuyenDi(string diemDon, string diemDen, DateTime TGDon, string loaiXe, string ghiChu, string IdKhachHang)
        {
            var newChuyenDi = new ChuyenDi
            {
                IdChuyenDi = Guid.NewGuid().ToString(),
                IdKH = IdKhachHang,
                DiemDen = diemDen,
                DiemDon = diemDon,
                ThoiGianTao = DateTime.Now,
                TrangThai = TrangThaiChuyen.MOI,
                LoaiXeYeuCau = (LoaiXe)Enum.Parse(typeof(LoaiXe), loaiXe),
                ThoiGianDon = TGDon,
                GhiChu = ghiChu
            };

            try
            {
                _context.ChuyenDis.Add(newChuyenDi);
                _context.SaveChanges();
                return 0; // Tạo chuyến đi thành công

            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần thiết
                return 1; // Lỗi khi lưu dữ liệu
            }
        }
    }
}
