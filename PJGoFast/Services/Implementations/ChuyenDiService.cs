using Microsoft.EntityFrameworkCore;
using PJGoFast.Data;
using PJGoFast.Models.Entities;
using PJGoFast.Models.Enums;
using PJGoFast.Services.Interfaces;
using PJGoFast.ViewModels;


namespace PJGoFast.Services.Implementations
{
    public class ChuyenDiService : IChuyenDiService
    {
        private readonly PJGoFastDbContext _context;

        

        public ChuyenDiService(PJGoFastDbContext context)
        {
            _context = context;
            
        }

        public string TaoChuyenDi(string diemDon, string diemDen, DateTime TGDon, LoaiXe loaiXe, string ghiChu, string IdKhachHang)
        {
            var newChuyenDi = new ChuyenDi
            {
                IdChuyenDi = Guid.NewGuid().ToString(),
                IdKH = IdKhachHang,
                DiemDen = diemDen,
                DiemDon = diemDon,
                ThoiGianTao = DateTime.Now,
                TrangThai = TrangThaiChuyen.MOI,
                LoaiXeYeuCau = loaiXe,
                ThoiGianDon = TGDon,
                GhiChu = ghiChu
            };

            try
            {
                _context.ChuyenDis.Add(newChuyenDi);
                _context.SaveChanges();
                return newChuyenDi.IdChuyenDi; // Tạo chuyến đi thành công

            }
            catch (Exception ex)
            {
                
                
                return "0"; // Lỗi khi lưu dữ liệu
            }
        }
    }
}
