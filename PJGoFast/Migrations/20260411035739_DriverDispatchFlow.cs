using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PJGoFast.Migrations
{
    /// <inheritdoc />
    public partial class DriverDispatchFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "HanNhanChuyen",
                table: "ChuyenDis",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HanNhanChuyen",
                table: "ChuyenDis");
        }
    }
}
