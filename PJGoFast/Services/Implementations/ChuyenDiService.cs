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
        private static readonly TrangThaiChuyen[] DriverBlockingStatuses =
        [
            TrangThaiChuyen.DA_NHAN,
            TrangThaiChuyen.DANG_DON,
            TrangThaiChuyen.DA_DON,
            TrangThaiChuyen.DANG_DI_CHUYEN
        ];

        private readonly PJGoFastDbContext _context;

        public ChuyenDiService(PJGoFastDbContext context) => _context = context;

        public string TaoChuyenDi(string diemDon, string diemDen, DateTime TGDon, LoaiXe loaiXe, string ghiChu, string IdKhachHang)
        {
            var chuyen = new ChuyenDi
            {
                IdChuyenDi = Guid.NewGuid().ToString(),
                IdKH = IdKhachHang,
                DiemDon = diemDon,
                DiemDen = diemDen,
                LoaiXeYeuCau = loaiXe,
                ThoiGianTao = DateTime.Now,
                ThoiGianDon = TGDon,
                GhiChu = ghiChu,
                TrangThai = TrangThaiChuyen.CHO
            };

            try
            {
                _context.ChuyenDis.Add(chuyen);
                AddLog(chuyen, string.Empty, TrangThaiChuyen.CHO.ToString(), "KH", "Khách hàng tạo yêu cầu và đang xem báo giá.");
                _context.SaveChanges();
                return chuyen.IdChuyenDi;
            }
            catch
            {
                return "0";
            }
        }

        public ChuyenDi? LayChuyenDiTheoId(string id)
        {
            XuLyPhanCongHetHan();

            return _context.ChuyenDis
                .Include(c => c.KhachHang)
                .Include(c => c.TaiXe)
                .Include(c => c.Admin)
                .Include(c => c.ThanhToan)
                .Include(c => c.NhatKys)
                .FirstOrDefault(c => c.IdChuyenDi == id);
        }

        public void CapNhatGiaTamTinh(string id, decimal gia)
        {
            var chuyen = _context.ChuyenDis.Find(id);
            if (chuyen == null)
            {
                return;
            }

            chuyen.GiaTamTinh = gia;
            _context.SaveChanges();
        }

        public List<ChuyenDi> LayDanhSachChuyenDiCuaKH(string idKH)
        {
            XuLyPhanCongHetHan();

            return _context.ChuyenDis
                .Include(c => c.TaiXe)
                .Include(c => c.Admin)
                .Include(c => c.ThanhToan)
                .Where(c => c.IdKH == idKH)
                .OrderByDescending(c => c.ThoiGianTao)
                .ToList();
        }

        public bool HuyChuyenDi(string id, string idKH)
        {
            XuLyPhanCongHetHan();

            var chuyen = _context.ChuyenDis
                .Include(c => c.TaiXe)
                .Include(c => c.ThanhToan)
                .FirstOrDefault(c => c.IdChuyenDi == id && c.IdKH == idKH);

            if (chuyen == null)
            {
                return false;
            }

            var coTheHuy = chuyen.TrangThai is TrangThaiChuyen.CHO or TrangThaiChuyen.MOI or TrangThaiChuyen.DA_PHAN_CONG;
            if (!coTheHuy)
            {
                return false;
            }

            var trangThaiCu = chuyen.TrangThai;
            chuyen.TrangThai = TrangThaiChuyen.HUY;
            chuyen.HanNhanChuyen = null;
            if (trangThaiCu == TrangThaiChuyen.DA_PHAN_CONG)
            {
                chuyen.IdTX = null;
            }

            AddLog(chuyen, trangThaiCu.ToString(), TrangThaiChuyen.HUY.ToString(), "KH", "Khách hàng hủy chuyến.");
            _context.SaveChanges();
            return true;
        }

        public bool XoaChuyenDi(string id, string idKH)
        {
            var chuyen = _context.ChuyenDis
                .Include(c => c.NhatKys)
                .Include(c => c.ThanhToan)
                .FirstOrDefault(c => c.IdChuyenDi == id && c.IdKH == idKH);

            if (chuyen == null || chuyen.TrangThai != TrangThaiChuyen.CHO)
            {
                return false;
            }

            if (chuyen.ThanhToan != null)
            {
                _context.ThanhToans.Remove(chuyen.ThanhToan);
            }

            if (chuyen.NhatKys.Any())
            {
                _context.nhatKys.RemoveRange(chuyen.NhatKys);
            }

            _context.ChuyenDis.Remove(chuyen);
            _context.SaveChanges();
            return true;
        }

        public bool XacNhanBaoGia(string id, string idKH)
        {
            var chuyen = _context.ChuyenDis.FirstOrDefault(c => c.IdChuyenDi == id && c.IdKH == idKH);
            if (chuyen == null)
            {
                return false;
            }

            if (chuyen.TrangThai == TrangThaiChuyen.MOI)
            {
                return true;
            }

            if (chuyen.TrangThai != TrangThaiChuyen.CHO)
            {
                return false;
            }

            chuyen.TrangThai = TrangThaiChuyen.MOI;
            AddLog(chuyen, TrangThaiChuyen.CHO.ToString(), TrangThaiChuyen.MOI.ToString(), "KH", "Khách hàng xác nhận báo giá và phát hành chuyến mới.");
            _context.SaveChanges();
            return true;
        }

        public TaiXeDashboardVM? LayBangDieuKhienTaiXe(string idTX)
        {
            XuLyPhanCongHetHan();

            var taiXe = _context.TaiXes.FirstOrDefault(t => t.IdTX == idTX);
            if (taiXe == null)
            {
                return null;
            }

            var activeTrip = GetDriverBlockingTrips(idTX)
                .OrderByDescending(c => c.ThoiGianTao)
                .FirstOrDefault();

            var assignedTrips = _context.ChuyenDis
                .Include(c => c.KhachHang)
                .Include(c => c.Admin)
                .Where(c => c.IdTX == idTX && c.TrangThai == TrangThaiChuyen.DA_PHAN_CONG)
                .OrderBy(c => c.HanNhanChuyen)
                .ToList();

            var dangBan = activeTrip != null || (assignedTrips.Any() && taiXe.TrangThaiOnline == TrangThaiOnline.BUSY);

            var chuyenMoi = _context.ChuyenDis
                .Include(c => c.KhachHang)
                .Where(c => c.TrangThai == TrangThaiChuyen.MOI && c.LoaiXeYeuCau == taiXe.LoaiXe)
                .OrderBy(c => c.ThoiGianDon)
                .ThenBy(c => c.ThoiGianTao)
                .Take(20)
                .ToList();

            var thongBao = _context.nhatKys
                .Where(n => n.LogText.Contains(idTX) || n.LogText.Contains("tài xế"))
                .OrderByDescending(n => n.ThoiGian)
                .Take(8)
                .Select(MapLog)
                .ToList();

            return new TaiXeDashboardVM
            {
                IdTX = taiXe.IdTX,
                HoVaTen = taiXe.HoVaTen,
                SDT = taiXe.SDT,
                ViTri = taiXe.ViTri,
                DiemDoi = taiXe.DiemDoi,
                LoaiXe = taiXe.LoaiXe.ToString(),
                TrangThaiOnline = taiXe.TrangThaiOnline,
                DangBan = activeTrip != null,
                ChuyenDangThucHien = activeTrip == null ? null : MapTrip(activeTrip, idTX, includeLogs: true),
                ChuyenDuocPhanCong = assignedTrips.Select(c => MapTrip(c, idTX, canReceive: !dangBan, canReject: true)).ToList(),
                ChuyenMoi = chuyenMoi.Select(c => MapTrip(c, idTX, canReceive: !dangBan)).ToList(),
                ThongBaoMoi = thongBao
            };
        }

        public TaiXeAccountVM? LayThongTinTaiKhoanTaiXe(string idTX)
        {
            XuLyPhanCongHetHan();

            var taiXe = _context.TaiXes
                .Include(t => t.ChuyenDis)
                .ThenInclude(c => c.ThanhToan)
                .FirstOrDefault(t => t.IdTX == idTX);

            if (taiXe == null)
            {
                return null;
            }

            var chuyenDangXuLy = taiXe.ChuyenDis.Count(c => IsDriverBlockedByTrip(c.TrangThai, c.ThanhToan != null));
            return new TaiXeAccountVM
            {
                IdTX = taiXe.IdTX,
                HoVaTen = taiXe.HoVaTen,
                SDT = taiXe.SDT,
                NgaySinh = taiXe.NgaySinh,
                TrangThaiHoatDong = taiXe.TrangThaiHoatDong,
                TrangThaiOnline = taiXe.TrangThaiOnline,
                LoaiXe = taiXe.LoaiXe,
                DiemDoi = taiXe.DiemDoi,
                ViTri = taiXe.ViTri,
                TongChuyen = taiXe.ChuyenDis.Count,
                ChuyenHoanTat = taiXe.ChuyenDis.Count(c => c.TrangThai == TrangThaiChuyen.HOAN_TAT),
                ChuyenDangXuLy = chuyenDangXuLy
            };
        }

        public TripSummaryVM? LayChiTietChuyen(string idChuyenDi)
        {
            XuLyPhanCongHetHan();

            var chuyen = _context.ChuyenDis
                .Include(c => c.KhachHang)
                .Include(c => c.TaiXe)
                .Include(c => c.Admin)
                .Include(c => c.ThanhToan)
                .Include(c => c.NhatKys)
                .FirstOrDefault(c => c.IdChuyenDi == idChuyenDi);

            return chuyen == null ? null : MapTrip(chuyen, includeLogs: true);
        }

        public DieuPhoiDashboardVM? LayBangDieuPhoi(string idAdmin)
        {
            XuLyPhanCongHetHan();

            var admin = _context.Admins.FirstOrDefault(a => a.IdAdmin == idAdmin);
            if (admin == null)
            {
                return null;
            }

            var trips = _context.ChuyenDis
                .Include(c => c.KhachHang)
                .Include(c => c.Admin)
                .Where(c => c.TrangThai == TrangThaiChuyen.MOI)
                .OrderBy(c => c.ThoiGianDon)
                .ThenBy(c => c.ThoiGianTao)
                .ToList()
                .Select(c => MapTrip(c))
                .ToList();

            var drivers = _context.TaiXes
                .Include(t => t.ChuyenDis)
                .ThenInclude(c => c.ThanhToan)
                .Where(t => t.TrangThaiOnline != TrangThaiOnline.OFFLINE)
                .AsEnumerable()
                .OrderBy(t => t.TrangThaiOnline == TrangThaiOnline.ONLINE ? 0 : 1)
                .ThenBy(t => t.HoVaTen)
                .Select(t =>
                {
                    var activeTrip = t.ChuyenDis
                        .Where(c => IsDriverBlockedByTrip(c.TrangThai, c.ThanhToan != null))
                        .OrderByDescending(c => c.ThoiGianTao)
                        .FirstOrDefault();

                    return new DieuPhoiDriverVM
                    {
                        IdTX = t.IdTX,
                        HoVaTen = t.HoVaTen,
                        SDT = t.SDT,
                        ViTri = t.ViTri,
                        DiemDoi = t.DiemDoi,
                        LoaiXe = t.LoaiXe.ToString(),
                        TrangThaiOnline = t.TrangThaiOnline,
                        DangBan = activeTrip != null,
                        ChuyenDangXuLy = activeTrip == null ? null : MapTrip(activeTrip, t.IdTX)
                    };
                })
                .ToList();

            var suKien = _context.nhatKys
                .OrderByDescending(n => n.ThoiGian)
                .Take(10)
                .Select(MapLog)
                .ToList();

            return new DieuPhoiDashboardVM
            {
                IdAdmin = admin.IdAdmin,
                HoVaTen = admin.HoVaTen,
                ChuyenMoi = trips,
                TaiXes = drivers,
                SuKienMoi = suKien
            };
        }

        public KhachHangTripTrackingVM LayChuyenDangTheoDoiChoKhach(string idKH)
        {
            XuLyPhanCongHetHan();

            var activeTrips = _context.ChuyenDis
                .Include(c => c.TaiXe)
                .Include(c => c.Admin)
                .Include(c => c.ThanhToan)
                .Include(c => c.NhatKys)
                .Where(c => c.IdKH == idKH && c.TrangThai != TrangThaiChuyen.HUY && c.TrangThai != TrangThaiChuyen.CHO)
                .OrderByDescending(c => c.ThoiGianTao)
                .ToList();

            var tripToTrack = activeTrips.FirstOrDefault(c => c.TrangThai != TrangThaiChuyen.HOAN_TAT || c.ThanhToan == null)
                ?? activeTrips.FirstOrDefault();

            return new KhachHangTripTrackingVM
            {
                ChuyenDangTheoDoi = tripToTrack == null ? null : MapTrip(tripToTrack, includeLogs: true),
                ChuyenDangXuLy = activeTrips.Select(c => MapTrip(c)).ToList(),
                Logs = tripToTrack?.NhatKys.OrderByDescending(n => n.ThoiGian).Select(MapLog).ToList() ?? []
            };
        }

        public List<TripLogItemVM> LayNhatKyChuyen(string idChuyenDi, string? idNguoiDung = null, string? vaiTro = null)
        {
            var chuyen = _context.ChuyenDis
                .Include(c => c.NhatKys)
                .FirstOrDefault(c => c.IdChuyenDi == idChuyenDi);

            if (chuyen == null)
            {
                return [];
            }

            if (!string.IsNullOrWhiteSpace(vaiTro) && !string.IsNullOrWhiteSpace(idNguoiDung))
            {
                var duocXem = vaiTro switch
                {
                    "KhachHang" => chuyen.IdKH == idNguoiDung,
                    "TaiXe" => chuyen.IdTX == idNguoiDung,
                    "QuanTri" => true,
                    "DieuPhoi" => true,
                    _ => false
                };

                if (!duocXem)
                {
                    return [];
                }
            }

            return chuyen.NhatKys
                .OrderByDescending(n => n.ThoiGian)
                .Select(MapLog)
                .ToList();
        }

        public (bool Success, string Message) PhanCongChuyen(string idChuyenDi, string idTX, string idAdmin)
        {
            XuLyPhanCongHetHan();

            var chuyen = _context.ChuyenDis
                .Include(c => c.KhachHang)
                .FirstOrDefault(c => c.IdChuyenDi == idChuyenDi);
            var taiXe = _context.TaiXes.FirstOrDefault(t => t.IdTX == idTX);

            if (chuyen == null || taiXe == null)
            {
                return (false, "Không tìm thấy chuyến đi hoặc tài xế.");
            }

            if (chuyen.TrangThai != TrangThaiChuyen.MOI)
            {
                return (false, "Chuyến đi này không còn ở trạng thái mới.");
            }

            if (taiXe.TrangThaiOnline == TrangThaiOnline.OFFLINE)
            {
                return (false, "Tài xế đang offline.");
            }

            if (HasBlockingTrip(idTX))
            {
                return (false, "Tài xế đang bận, không thể phân công thêm.");
            }

            if (taiXe.LoaiXe != chuyen.LoaiXeYeuCau)
            {
                return (false, "Loại xe của tài xế không phù hợp với chuyến đi.");
            }

            chuyen.TrangThai = TrangThaiChuyen.DA_PHAN_CONG;
            chuyen.IdTX = idTX;
            chuyen.IdAdmin = idAdmin;
            chuyen.HanNhanChuyen = DateTime.Now.AddMinutes(2);
            AddLog(chuyen, TrangThaiChuyen.MOI.ToString(), TrangThaiChuyen.DA_PHAN_CONG.ToString(), "Admin", $"Điều phối phân công chuyến cho tài xế {taiXe.HoVaTen} ({taiXe.IdTX}).");
            _context.SaveChanges();

            return (true, "Đã phân công chuyến đi.");
        }

        public (bool Success, string Message, string? RedirectTripId) NhanChuyen(string idChuyenDi, string idTX)
        {
            XuLyPhanCongHetHan();

            var chuyen = _context.ChuyenDis
                .Include(c => c.TaiXe)
                .Include(c => c.ThanhToan)
                .FirstOrDefault(c => c.IdChuyenDi == idChuyenDi);
            var taiXe = _context.TaiXes.FirstOrDefault(t => t.IdTX == idTX);

            if (chuyen == null || taiXe == null)
            {
                return (false, "Không tìm thấy chuyến đi hoặc tài xế.", null);
            }

            if (HasBlockingTrip(idTX, idChuyenDi))
            {
                return (false, "Tài xế chỉ được nhận một chuyến tại một thời điểm.", null);
            }

            var coTheNhan = chuyen.TrangThai == TrangThaiChuyen.MOI
                || (chuyen.TrangThai == TrangThaiChuyen.DA_PHAN_CONG && chuyen.IdTX == idTX && (!chuyen.HanNhanChuyen.HasValue || chuyen.HanNhanChuyen >= DateTime.Now));

            if (!coTheNhan)
            {
                return (false, "Chuyến đi không còn khả dụng để nhận.", null);
            }

            var trangThaiCu = chuyen.TrangThai;
            chuyen.TrangThai = TrangThaiChuyen.DA_NHAN;
            chuyen.IdTX = idTX;
            chuyen.HanNhanChuyen = null;
            taiXe.TrangThaiOnline = TrangThaiOnline.BUSY;
            AddLog(chuyen, trangThaiCu.ToString(), TrangThaiChuyen.DA_NHAN.ToString(), "TX", $"Tài xế {taiXe.HoVaTen} nhận chuyến.");
            _context.SaveChanges();

            return (true, "Đã nhận chuyến, chuyển sang cập nhật tiến trình.", chuyen.IdChuyenDi);
        }

        public (bool Success, string Message) TuChoiChuyenDuocPhanCong(string idChuyenDi, string idTX, bool tuDong = false)
        {
            var chuyen = _context.ChuyenDis.FirstOrDefault(c => c.IdChuyenDi == idChuyenDi);
            if (chuyen == null || chuyen.TrangThai != TrangThaiChuyen.DA_PHAN_CONG || chuyen.IdTX != idTX)
            {
                return (false, "Chuyến đi không còn ở trạng thái được phân công.");
            }

            var taiXe = _context.TaiXes.FirstOrDefault(t => t.IdTX == idTX);
            var trangThaiCu = chuyen.TrangThai;
            chuyen.TrangThai = TrangThaiChuyen.MOI;
            chuyen.IdTX = null;
            chuyen.HanNhanChuyen = null;
            AddLog(
                chuyen,
                trangThaiCu.ToString(),
                TrangThaiChuyen.MOI.ToString(),
                "TX",
                tuDong
                    ? $"Hết thời gian phản hồi. Hệ thống tự động từ chối phân công của tài xế {taiXe?.HoVaTen ?? idTX} và trả chuyến về trạng thái mới."
                    : $"Tài xế {taiXe?.HoVaTen ?? idTX} từ chối chuyến được phân công. Điều phối cần xử lý lại.");
            _context.SaveChanges();
            return (true, "Đã từ chối chuyến được phân công.");
        }

        public (bool Success, string Message) CapNhatTienTrinhTaiXe(string idChuyenDi, string idTX, TrangThaiChuyen trangThaiMoi)
        {
            XuLyPhanCongHetHan();

            var chuyen = _context.ChuyenDis
                .Include(c => c.ThanhToan)
                .FirstOrDefault(c => c.IdChuyenDi == idChuyenDi && c.IdTX == idTX);

            if (chuyen == null)
            {
                return (false, "Không tìm thấy chuyến đi.");
            }

            var trangThaiKeTiep = chuyen.TrangThai switch
            {
                TrangThaiChuyen.DA_NHAN => TrangThaiChuyen.DANG_DON,
                TrangThaiChuyen.DANG_DON => TrangThaiChuyen.DA_DON,
                TrangThaiChuyen.DA_DON => TrangThaiChuyen.DANG_DI_CHUYEN,
                TrangThaiChuyen.DANG_DI_CHUYEN => TrangThaiChuyen.HOAN_TAT,
                _ => (TrangThaiChuyen?)null
            };

            if (trangThaiKeTiep == null || trangThaiKeTiep != trangThaiMoi)
            {
                return (false, "Không thể cập nhật nhảy cóc trạng thái.");
            }

            var trangThaiCu = chuyen.TrangThai;
            chuyen.TrangThai = trangThaiMoi;

            var logText = trangThaiMoi switch
            {
                TrangThaiChuyen.DANG_DON => "Tài xế bắt đầu di chuyển đến điểm đón.",
                TrangThaiChuyen.DA_DON => "Tài xế xác nhận đã đón khách.",
                TrangThaiChuyen.DANG_DI_CHUYEN => "Tài xế bắt đầu hành trình đến điểm đến.",
                TrangThaiChuyen.HOAN_TAT => "Tài xế xác nhận đã hoàn tất chuyến đi.",
                _ => "Tài xế cập nhật trạng thái chuyến đi."
            };

            AddLog(chuyen, trangThaiCu.ToString(), trangThaiMoi.ToString(), "TX", logText);
            _context.SaveChanges();
            return (true, "Đã cập nhật tiến trình chuyến đi.");
        }

        public (bool Success, string Message) HuyChuyenTuTaiXe(string idChuyenDi, string idTX)
        {
            var chuyen = _context.ChuyenDis.FirstOrDefault(c => c.IdChuyenDi == idChuyenDi && c.IdTX == idTX);
            var taiXe = _context.TaiXes.FirstOrDefault(t => t.IdTX == idTX);

            if (chuyen == null || taiXe == null)
            {
                return (false, "Không tìm thấy chuyến đi hoặc tài xế.");
            }

            if (chuyen.TrangThai == TrangThaiChuyen.HOAN_TAT || chuyen.TrangThai == TrangThaiChuyen.HUY)
            {
                return (false, "Không thể hủy chuyến ở trạng thái hiện tại.");
            }

            var trangThaiCu = chuyen.TrangThai;
            chuyen.TrangThai = TrangThaiChuyen.HUY;
            chuyen.HanNhanChuyen = null;
            taiXe.TrangThaiOnline = TrangThaiOnline.ONLINE;
            AddLog(chuyen, trangThaiCu.ToString(), TrangThaiChuyen.HUY.ToString(), "TX", "Tài xế hủy chuyến.");
            _context.SaveChanges();
            return (true, "Đã hủy chuyến đi.");
        }

        public (bool Success, string Message) XacNhanThanhToan(string idChuyenDi, string idTX, string phuongThucThanhToan, decimal soTienThanhToan)
        {
            var chuyen = _context.ChuyenDis
                .Include(c => c.ThanhToan)
                .FirstOrDefault(c => c.IdChuyenDi == idChuyenDi && c.IdTX == idTX);
            var taiXe = _context.TaiXes.FirstOrDefault(t => t.IdTX == idTX);

            if (chuyen == null || taiXe == null)
            {
                return (false, "Không tìm thấy chuyến đi hoặc tài xế.");
            }

            if (chuyen.TrangThai != TrangThaiChuyen.HOAN_TAT)
            {
                return (false, "Chỉ xác nhận thanh toán sau khi hoàn tất chuyến.");
            }

            if (chuyen.ThanhToan != null)
            {
                return (false, "Chuyến đi này đã có xác nhận thanh toán.");
            }

            var thanhToan = new ThanhToan
            {
                IdThanhToan = Guid.NewGuid().ToString(),
                IdChuyenDi = chuyen.IdChuyenDi,
                TrangThaiThanhToan = "DaThanhToan",
                PhuongThucThanhToan = phuongThucThanhToan,
                SoTienThanhToan = soTienThanhToan,
                ThoiGianGhiNhan = DateTime.Now
            };

            if (soTienThanhToan > 0)
            {
                chuyen.GiaThucTe = soTienThanhToan;
            }

            _context.ThanhToans.Add(thanhToan);
            taiXe.TrangThaiOnline = TrangThaiOnline.ONLINE;
            AddLog(chuyen, chuyen.TrangThai.ToString(), chuyen.TrangThai.ToString(), "TX", $"Tài xế xác nhận thanh toán qua {phuongThucThanhToan} với số tiền {soTienThanhToan:N0} đ.");
            _context.SaveChanges();
            return (true, "Đã ghi nhận thanh toán.");
        }

        private IEnumerable<ChuyenDi> GetDriverBlockingTrips(string idTX)
        {
            return _context.ChuyenDis
                .Include(c => c.KhachHang)
                .Include(c => c.Admin)
                .Include(c => c.TaiXe)
                .Include(c => c.ThanhToan)
                .Include(c => c.NhatKys)
                .Where(c => c.IdTX == idTX)
                .AsEnumerable()
                .Where(c => IsDriverBlockedByTrip(c.TrangThai, c.ThanhToan != null));
        }

        private bool HasBlockingTrip(string idTX, string? excludeTripId = null)
        {
            return _context.ChuyenDis
                .Include(c => c.ThanhToan)
                .Where(c => c.IdTX == idTX && c.IdChuyenDi != excludeTripId)
                .AsEnumerable()
                .Any(c => IsDriverBlockedByTrip(c.TrangThai, c.ThanhToan != null));
        }

        private static bool IsDriverBlockedByTrip(TrangThaiChuyen trangThai, bool daCoThanhToan)
        {
            if (DriverBlockingStatuses.Contains(trangThai))
            {
                return true;
            }

            return trangThai == TrangThaiChuyen.HOAN_TAT && !daCoThanhToan;
        }

        private void XuLyPhanCongHetHan()
        {
            var now = DateTime.Now;
            var expiredTrips = _context.ChuyenDis
                .Where(c => c.TrangThai == TrangThaiChuyen.DA_PHAN_CONG && c.HanNhanChuyen.HasValue && c.HanNhanChuyen < now)
                .ToList();

            if (!expiredTrips.Any())
            {
                return;
            }

            foreach (var trip in expiredTrips)
            {
                var oldStatus = trip.TrangThai;
                var oldDriverId = trip.IdTX;
                trip.TrangThai = TrangThaiChuyen.MOI;
                trip.IdTX = null;
                trip.HanNhanChuyen = null;
                AddLog(trip, oldStatus.ToString(), TrangThaiChuyen.MOI.ToString(), "System", $"Hết thời gian nhận chuyến. Hệ thống tự động từ chối phân công của tài xế {oldDriverId ?? "không xác định"}.");
            }

            _context.SaveChanges();
        }

        private void AddLog(ChuyenDi chuyen, string trangThaiCu, string trangThaiMoi, string thucHienBoi, string logText)
        {
            _context.nhatKys.Add(new NhatKy
            {
                IdNhatKy = Guid.NewGuid().ToString(),
                IdChuyenDi = chuyen.IdChuyenDi,
                TrangThaiCu = trangThaiCu,
                TrangThaiMoi = trangThaiMoi,
                ThucHienBoi = thucHienBoi,
                ThoiGian = DateTime.Now,
                LogText = logText
            });
        }

        private static TripLogItemVM MapLog(NhatKy log)
        {
            return new TripLogItemVM
            {
                IdNhatKy = log.IdNhatKy,
                TrangThaiCu = log.TrangThaiCu,
                TrangThaiMoi = log.TrangThaiMoi,
                ThucHienBoi = log.ThucHienBoi,
                ThoiGian = log.ThoiGian,
                LogText = log.LogText
            };
        }

        private TripSummaryVM MapTrip(ChuyenDi chuyen, string? currentDriverId = null, bool canReceive = false, bool canReject = false, bool includeLogs = false)
        {
            var remainingSeconds = chuyen.HanNhanChuyen.HasValue
                ? Math.Max(0, (int)Math.Floor((chuyen.HanNhanChuyen.Value - DateTime.Now).TotalSeconds))
                : (int?)null;

            return new TripSummaryVM
            {
                IdChuyenDi = chuyen.IdChuyenDi,
                IdKH = chuyen.IdKH,
                IdTX = chuyen.IdTX,
                IdAdmin = chuyen.IdAdmin,
                DiemDon = chuyen.DiemDon,
                DiemDen = chuyen.DiemDen,
                LoaiXeYeuCau = chuyen.LoaiXeYeuCau,
                TrangThai = chuyen.TrangThai,
                ThoiGianTao = chuyen.ThoiGianTao,
                ThoiGianDon = chuyen.ThoiGianDon,
                HanNhanChuyen = chuyen.HanNhanChuyen,
                GhiChu = chuyen.GhiChu,
                GiaTamTinh = chuyen.GiaTamTinh,
                GiaThucTe = chuyen.GiaThucTe,
                RemainingSeconds = remainingSeconds,
                IsAssignedToCurrentDriver = !string.IsNullOrWhiteSpace(currentDriverId) && chuyen.IdTX == currentDriverId,
                CanReceive = canReceive,
                CanReject = canReject,
                CanCancel = chuyen.TrangThai is TrangThaiChuyen.CHO or TrangThaiChuyen.MOI or TrangThaiChuyen.DA_PHAN_CONG,
                HasPayment = chuyen.ThanhToan != null,
                PhuongThucThanhToan = chuyen.ThanhToan?.PhuongThucThanhToan,
                SoTienThanhToan = chuyen.ThanhToan?.SoTienThanhToan,
                KhachHang = chuyen.KhachHang == null ? null : new TripParticipantVM
                {
                    Id = chuyen.KhachHang.IdKH,
                    HoVaTen = chuyen.KhachHang.HoVaTen,
                    SDT = chuyen.KhachHang.SDT,
                    VaiTro = "KhachHang"
                },
                TaiXe = chuyen.TaiXe == null ? null : new TripParticipantVM
                {
                    Id = chuyen.TaiXe.IdTX,
                    HoVaTen = chuyen.TaiXe.HoVaTen,
                    SDT = chuyen.TaiXe.SDT,
                    VaiTro = "TaiXe",
                    ViTri = chuyen.TaiXe.ViTri,
                    DiemDoi = chuyen.TaiXe.DiemDoi,
                    LoaiXe = chuyen.TaiXe.LoaiXe.ToString(),
                    TrangThaiOnline = chuyen.TaiXe.TrangThaiOnline.ToString()
                },
                Admin = chuyen.Admin == null ? null : new TripParticipantVM
                {
                    Id = chuyen.Admin.IdAdmin,
                    HoVaTen = chuyen.Admin.HoVaTen,
                    SDT = chuyen.Admin.SDT,
                    VaiTro = chuyen.Admin.VaiTro
                },
                Logs = includeLogs && chuyen.NhatKys != null
                    ? chuyen.NhatKys.OrderByDescending(n => n.ThoiGian).Select(MapLog).ToList()
                    : []
            };
        }
    }
}
