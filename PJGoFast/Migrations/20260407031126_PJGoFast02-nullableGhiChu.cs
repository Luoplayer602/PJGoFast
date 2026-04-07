using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PJGoFast.Migrations
{
    /// <inheritdoc />
    public partial class PJGoFast02nullableGhiChu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "GiaTamTinh",
                table: "ChuyenDis",
                type: "decimal(18,0)",
                precision: 18,
                scale: 0,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldPrecision: 18);

            migrationBuilder.AlterColumn<string>(
                name: "GhiChu",
                table: "ChuyenDis",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "GiaTamTinh",
                table: "ChuyenDis",
                type: "decimal(18,0)",
                precision: 18,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldPrecision: 18,
                oldScale: 0,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GhiChu",
                table: "ChuyenDis",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
