using System.ComponentModel.DataAnnotations;

namespace PJGoFast.Models.Enums
{
    public enum TrangThaiChuyen
    {
        MOI = 0,
        DA_PHAN_CONG = 1,
        DA_NHAN = 2,
        DANG_DON = 3,
        DA_DON = 4,
        DANG_DI_CHUYEN = 5,
        HOAN_TAT = 6,
        HUY = 7,
        CHO = 8
    }

    public enum TrangThaiOnline
    {
        ONLINE = 0,
        BUSY = 1,
        OFFLINE = 2
    }

    public enum LoaiXe
    {
        [Display(Name = "Xe máy")]
        XeMay = 0,
        [Display(Name = "Xe sedan")]
        Sedan = 1,
        [Display(Name = "Xe SUV")]
        SUV = 2,
        [Display(Name = "Xe bảy chỗ")]
        BayCho = 3,
        [Display(Name = "Xe tải nhỏ")]
        TaiNho = 4
    }
}
