using PJGoFast.Models.Entities;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using PJGoFast.Models.Enums;

namespace PJGoFast.Data
{
    public class PJGoFastDbContext : DbContext
    {
        public PJGoFastDbContext(DbContextOptions<PJGoFastDbContext> options) : base(options)
        {
        }

        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<TaiXe> TaiXes { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<ChuyenDi> ChuyenDis { get; set; }
        public DbSet<NhatKy> nhatKys { get; set; }
        public DbSet<ThanhToan> ThanhToans { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            //enum -> string
            mb.Entity<ChuyenDi>()
                .Property(c => c.TrangThai)
                .HasConversion<string>();

            mb.Entity<ChuyenDi>()
                .Property(c => c.LoaiXeYeuCau)
                .HasConversion<string>();
            
            mb.Entity<TaiXe>()
                .Property(t => t.LoaiXe)
                .HasConversion<string>();

            mb.Entity<TaiXe>()
                .Property(t => t.TrangThaiOnline)
                .HasConversion<string>();

            //Decimal precision
            mb.Entity<ChuyenDi>()
                .Property(c => c.GiaTamTinh)
                .HasPrecision(18, 0);

            mb.Entity<ChuyenDi>()
                .Property(c => c.GiaThucTe)
                .HasPrecision(18, 0);

            mb.Entity<ThanhToan>()
                .Property(t => t.SoTienThanhToan)
                .HasPrecision(18, 0);

            // 1-1 ChuyenDi - ThanhToan
            mb.Entity<ChuyenDi>()
                .HasOne(c => c.ThanhToan)
                .WithOne(t => t.ChuyenDi)
                .HasForeignKey<ThanhToan>(t => t.IdChuyenDi);

            // ChuyenDi -> Admin(nullable) 1-n
            mb.Entity<ChuyenDi>()
                .HasOne(c => c.Admin)
                .WithMany(a => a.ChuyenDis)
                .HasForeignKey(c => c.IdAdmin)
                .IsRequired(false);

            // ChuyenDi -> TaiXe(nullable) 1-n
            mb.Entity<ChuyenDi>()
                .HasOne(c => c.TaiXe)
                .WithMany(t => t.ChuyenDis)
                .HasForeignKey(c => c.IdTX)
                .IsRequired(false);

            //index
            mb.Entity<ChuyenDi>()
                .HasIndex(c => new { c.TrangThai, c.ThoiGianTao });

            mb.Entity<NhatKy>()
                .HasIndex(n => new { n.IdChuyenDi, n.ThoiGian });

            mb.Entity<TaiXe>()
                .HasIndex(t => t.TrangThaiOnline);

            mb.Entity<KhachHang>()
                .HasIndex(k => k.SDT)
                .IsUnique();

            mb.Entity<TaiXe>()
                .HasIndex(k => k.SDT)
                .IsUnique();

            mb.Entity<Admin>()
                .HasIndex(a => a.SDT)
                .IsUnique();

            //seed admin mac dinh
            mb.Entity<Admin>().HasData(new Admin
            {
                IdAdmin = "TAOLACHUA",
                HoVaTen = "Quản Trị Viên Mặc Định",
                SDT = "0987654321",
                MatKhau = "$2y$10$AwT9qSEsKIrmPBEKGEY0geoqHAvYpgAkUU0a97AEF6y4iMTyN.LVG",
                VaiTro = "QuanTri",
                NgaySinh = new DateTime(2000,1,1)
            });
        }
    }
}
