using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PJGoFast.Migrations
{
    /// <inheritdoc />
    public partial class PJGoFast0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    IdAdmin = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HoVaTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VaiTro = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.IdAdmin);
                });

            migrationBuilder.CreateTable(
                name: "KhachHangs",
                columns: table => new
                {
                    IdKH = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HoVaTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayDangKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHangs", x => x.IdKH);
                });

            migrationBuilder.CreateTable(
                name: "TaiXes",
                columns: table => new
                {
                    IdTX = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HoVaTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThaiHoatDong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiXe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiemDoi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ViTri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThaiOnline = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiXes", x => x.IdTX);
                });

            migrationBuilder.CreateTable(
                name: "ChuyenDis",
                columns: table => new
                {
                    IdChuyenDi = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdKH = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdTX = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IdAdmin = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DiemDon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiemDen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiXeYeuCau = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThoiGianTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianDon = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GiaTamTinh = table.Column<decimal>(type: "decimal(18,0)", precision: 18, scale: 0, nullable: false),
                    GiaThucTe = table.Column<decimal>(type: "decimal(18,0)", precision: 18, scale: 0, nullable: true),
                    KhachHangIdKH = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChuyenDis", x => x.IdChuyenDi);
                    table.ForeignKey(
                        name: "FK_ChuyenDis_Admins_IdAdmin",
                        column: x => x.IdAdmin,
                        principalTable: "Admins",
                        principalColumn: "IdAdmin");
                    table.ForeignKey(
                        name: "FK_ChuyenDis_KhachHangs_KhachHangIdKH",
                        column: x => x.KhachHangIdKH,
                        principalTable: "KhachHangs",
                        principalColumn: "IdKH",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChuyenDis_TaiXes_IdTX",
                        column: x => x.IdTX,
                        principalTable: "TaiXes",
                        principalColumn: "IdTX");
                });

            migrationBuilder.CreateTable(
                name: "nhatKys",
                columns: table => new
                {
                    IdNhatKy = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdChuyenDi = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TrangThaiCu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThaiMoi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThucHienBoi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThoiGian = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LogText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChuyenDiIdChuyenDi = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nhatKys", x => x.IdNhatKy);
                    table.ForeignKey(
                        name: "FK_nhatKys_ChuyenDis_ChuyenDiIdChuyenDi",
                        column: x => x.ChuyenDiIdChuyenDi,
                        principalTable: "ChuyenDis",
                        principalColumn: "IdChuyenDi");
                });

            migrationBuilder.CreateTable(
                name: "ThanhToans",
                columns: table => new
                {
                    IdThanhToan = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdChuyenDi = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TrangThaiThanhToan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhuongThucThanhToan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoTienThanhToan = table.Column<decimal>(type: "decimal(18,0)", precision: 18, scale: 0, nullable: false),
                    ThoiGianGhiNhan = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhToans", x => x.IdThanhToan);
                    table.ForeignKey(
                        name: "FK_ThanhToans_ChuyenDis_IdChuyenDi",
                        column: x => x.IdChuyenDi,
                        principalTable: "ChuyenDis",
                        principalColumn: "IdChuyenDi",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "IdAdmin", "HoVaTen", "MatKhau", "NgaySinh", "SDT", "VaiTro" },
                values: new object[] { "TAOLACHUA", "Quản Trị Viên Mặc Định", "$2a$11$QZLza5JFper7Uml90H4KPOGVtgQwJYk3inBjAci.zIshvJaLg3WfC", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "0987654321", "QuanTri" });

            migrationBuilder.CreateIndex(
                name: "IX_Admins_SDT",
                table: "Admins",
                column: "SDT",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChuyenDis_IdAdmin",
                table: "ChuyenDis",
                column: "IdAdmin");

            migrationBuilder.CreateIndex(
                name: "IX_ChuyenDis_IdTX",
                table: "ChuyenDis",
                column: "IdTX");

            migrationBuilder.CreateIndex(
                name: "IX_ChuyenDis_KhachHangIdKH",
                table: "ChuyenDis",
                column: "KhachHangIdKH");

            migrationBuilder.CreateIndex(
                name: "IX_ChuyenDis_TrangThai_ThoiGianTao",
                table: "ChuyenDis",
                columns: new[] { "TrangThai", "ThoiGianTao" });

            migrationBuilder.CreateIndex(
                name: "IX_KhachHangs_SDT",
                table: "KhachHangs",
                column: "SDT",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_nhatKys_ChuyenDiIdChuyenDi",
                table: "nhatKys",
                column: "ChuyenDiIdChuyenDi");

            migrationBuilder.CreateIndex(
                name: "IX_nhatKys_IdChuyenDi_ThoiGian",
                table: "nhatKys",
                columns: new[] { "IdChuyenDi", "ThoiGian" });

            migrationBuilder.CreateIndex(
                name: "IX_TaiXes_SDT",
                table: "TaiXes",
                column: "SDT",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaiXes_TrangThaiOnline",
                table: "TaiXes",
                column: "TrangThaiOnline");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToans_IdChuyenDi",
                table: "ThanhToans",
                column: "IdChuyenDi",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "nhatKys");

            migrationBuilder.DropTable(
                name: "ThanhToans");

            migrationBuilder.DropTable(
                name: "ChuyenDis");

            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "KhachHangs");

            migrationBuilder.DropTable(
                name: "TaiXes");
        }
    }
}
