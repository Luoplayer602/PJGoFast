using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PJGoFast.Migrations
{
    /// <inheritdoc />
    public partial class PJGoFast03addFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChuyenDis_KhachHangs_KhachHangIdKH",
                table: "ChuyenDis");

            migrationBuilder.DropForeignKey(
                name: "FK_nhatKys_ChuyenDis_ChuyenDiIdChuyenDi",
                table: "nhatKys");

            migrationBuilder.DropIndex(
                name: "IX_nhatKys_ChuyenDiIdChuyenDi",
                table: "nhatKys");

            migrationBuilder.DropIndex(
                name: "IX_ChuyenDis_KhachHangIdKH",
                table: "ChuyenDis");

            migrationBuilder.DropColumn(
                name: "ChuyenDiIdChuyenDi",
                table: "nhatKys");

            migrationBuilder.DropColumn(
                name: "KhachHangIdKH",
                table: "ChuyenDis");

            migrationBuilder.AlterColumn<string>(
                name: "IdKH",
                table: "ChuyenDis",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ChuyenDis_IdKH",
                table: "ChuyenDis",
                column: "IdKH");

            migrationBuilder.AddForeignKey(
                name: "FK_ChuyenDis_KhachHangs_IdKH",
                table: "ChuyenDis",
                column: "IdKH",
                principalTable: "KhachHangs",
                principalColumn: "IdKH",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_nhatKys_ChuyenDis_IdChuyenDi",
                table: "nhatKys",
                column: "IdChuyenDi",
                principalTable: "ChuyenDis",
                principalColumn: "IdChuyenDi",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChuyenDis_KhachHangs_IdKH",
                table: "ChuyenDis");

            migrationBuilder.DropForeignKey(
                name: "FK_nhatKys_ChuyenDis_IdChuyenDi",
                table: "nhatKys");

            migrationBuilder.DropIndex(
                name: "IX_ChuyenDis_IdKH",
                table: "ChuyenDis");

            migrationBuilder.AddColumn<string>(
                name: "ChuyenDiIdChuyenDi",
                table: "nhatKys",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IdKH",
                table: "ChuyenDis",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "KhachHangIdKH",
                table: "ChuyenDis",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_nhatKys_ChuyenDiIdChuyenDi",
                table: "nhatKys",
                column: "ChuyenDiIdChuyenDi");

            migrationBuilder.CreateIndex(
                name: "IX_ChuyenDis_KhachHangIdKH",
                table: "ChuyenDis",
                column: "KhachHangIdKH");

            migrationBuilder.AddForeignKey(
                name: "FK_ChuyenDis_KhachHangs_KhachHangIdKH",
                table: "ChuyenDis",
                column: "KhachHangIdKH",
                principalTable: "KhachHangs",
                principalColumn: "IdKH",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_nhatKys_ChuyenDis_ChuyenDiIdChuyenDi",
                table: "nhatKys",
                column: "ChuyenDiIdChuyenDi",
                principalTable: "ChuyenDis",
                principalColumn: "IdChuyenDi");
        }
    }
}
