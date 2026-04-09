using Microsoft.EntityFrameworkCore;
using PJGoFast.Data;
using PJGoFast.Models.Entities;
using PJGoFast.Models.Enums;
using PJGoFast.Services.Interfaces;
using PJGoFast.ViewModels;

// NOTE: Thêm 2 method sau vào interface IChuyenDiService:
//   ChuyenDi? LayChuyenDiTheoId(string id);
//   void CapNhatGiaTamTinh(string id, decimal gia);

namespace PJGoFast.Services.Implementations
{
    public class ChuyenDiService : IChuyenDiService
    {
        private readonly PJGoFastDbContext _context;

        public ChuyenDiService(PJGoFastDbContext context)
        {
            _context = context;
        }

        /// <summary>Tạo chuyến đi mới. Trả về IdChuyenDi nếu thành công, "0" nếu lỗi.</summary>
        public string TaoChuyenDi(string diemDon, string diemDen, DateTime TGDon,
                                   LoaiXe loaiXe, string ghiChu, string IdKhachHang)
        {
            var newChuyenDi = new ChuyenDi
            {
                IdChuyenDi   = Guid.NewGuid().ToString(),
                IdKH         = IdKhachHang,
                DiemDen      = diemDen,
                DiemDon      = diemDon,
                ThoiGianTao  = DateTime.Now,
                TrangThai    = TrangThaiChuyen.MOI,
                LoaiXeYeuCau = loaiXe,
                ThoiGianDon  = TGDon,
                GhiChu       = ghiChu
            };

            try
            {
                _context.ChuyenDis.Add(newChuyenDi);
                _context.SaveChanges();
                return newChuyenDi.IdChuyenDi;
            }
            catch
            {
                return "0";
            }
        }

        /// <summary>Lấy thông tin chuyến đi theo Id. Trả về null nếu không tìm thấy.</summary>
        public ChuyenDi? LayChuyenDiTheoId(string id)
        {
            return _context.ChuyenDis
                .Include(c => c.KhachHang)   // include nếu cần hiển thị tên KH
                .FirstOrDefault(c => c.IdChuyenDi == id);
        }

        public List<ChuyenDi> LayLichSuChuyenDiTheoKhachHang(string idKhachHang)
        {
            return _context.ChuyenDis
                .Where(c => c.IdKH == idKhachHang)
                .OrderByDescending(c => c.ThoiGianTao)
                .ToList();
        }

        /// <summary>Cập nhật giá tạm tính sau khi tính toán từ Goong Distance Matrix.</summary>
        public void CapNhatGiaTamTinh(string id, decimal gia)
        {
            var cd = _context.ChuyenDis.Find(id);
            if (cd == null) return;

            cd.GiaTamTinh = gia;
            _context.SaveChanges();
        }
    }
}
