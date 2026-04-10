using Microsoft.EntityFrameworkCore;
using PJGoFast.Data;
using PJGoFast.Models.Entities;
using PJGoFast.Models.Enums;
using PJGoFast.Services.Interfaces;
using PJGoFast.ViewModels;

// NOTE: Thêm các method sau vào interface IChuyenDiService:
//   ChuyenDi?              LayChuyenDiTheoId(string id);
//   void                   CapNhatGiaTamTinh(string id, decimal gia);
//   List<ChuyenDi>         LayDanhSachChuyenDiCuaKH(string idKH);
//   bool                   HuyChuyenDi(string id, string idKH);
//   bool                   XoaChuyenDi(string id, string idKH);

namespace PJGoFast.Services.Implementations
{
    public class ChuyenDiService : IChuyenDiService
    {
        private readonly PJGoFastDbContext _context;

        public ChuyenDiService(PJGoFastDbContext context) => _context = context;

        /// <summary>Tạo chuyến đi mới. Trả về IdChuyenDi nếu thành công, "0" nếu lỗi.</summary>
        public string TaoChuyenDi(string diemDon, string diemDen, DateTime TGDon,
                                   LoaiXe loaiXe, string ghiChu, string IdKhachHang)
        {
            var cd = new ChuyenDi
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
                _context.ChuyenDis.Add(cd);
                _context.SaveChanges();
                return cd.IdChuyenDi;
            }
            catch { return "0"; }
        }

        /// <summary>Lấy thông tin chuyến đi theo Id.</summary>
        public ChuyenDi? LayChuyenDiTheoId(string id)
            => _context.ChuyenDis
                .Include(c => c.KhachHang)
                .FirstOrDefault(c => c.IdChuyenDi == id);

        /// <summary>Lấy toàn bộ lịch sử chuyến đi của một khách hàng, mới nhất trước.</summary>
        public List<ChuyenDi> LayDanhSachChuyenDiCuaKH(string idKH)
            => _context.ChuyenDis
                .Where(c => c.IdKH == idKH)
                .OrderByDescending(c => c.ThoiGianTao)
                .ToList();

        /// <summary>Cập nhật giá tạm tính.</summary>
        public void CapNhatGiaTamTinh(string id, decimal gia)
        {
            var cd = _context.ChuyenDis.Find(id);
            if (cd == null) return;
            cd.GiaTamTinh = gia;
            _context.SaveChanges();
        }

        /// <summary>
        /// Hủy chuyến đi (chuyển trạng thái → HUY).
        /// Chỉ áp dụng khi chuyến đang ở trạng thái MOI và thuộc về idKH.
        /// Trả về true nếu thành công.
        /// </summary>
        public bool HuyChuyenDi(string id, string idKH)
        {
            var cd = _context.ChuyenDis.Find(id);
            if (cd == null || cd.IdKH != idKH || cd.TrangThai != TrangThaiChuyen.MOI)
                return false;
            cd.TrangThai = TrangThaiChuyen.HUY;
            _context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Xóa hẳn chuyến đi khỏi DB.
        /// Chỉ áp dụng khi chuyến đang ở trạng thái MOI và thuộc về idKH.
        /// Trả về true nếu thành công.
        /// </summary>
        public bool XoaChuyenDi(string id, string idKH)
        {
            var cd = _context.ChuyenDis.Find(id);
            if (cd == null || cd.IdKH != idKH || cd.TrangThai != TrangThaiChuyen.MOI)
                return false;
            _context.ChuyenDis.Remove(cd);
            _context.SaveChanges();
            return true;
        }
    }
}
