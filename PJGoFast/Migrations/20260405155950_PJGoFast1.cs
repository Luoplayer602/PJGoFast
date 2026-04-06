using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PJGoFast.Migrations
{
    /// <inheritdoc />
    public partial class PJGoFast1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "IdAdmin",
                keyValue: "TAOLACHUA",
                column: "MatKhau",
                value: "$2y$10$AwT9qSEsKIrmPBEKGEY0geoqHAvYpgAkUU0a97AEF6y4iMTyN.LVG");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "IdAdmin",
                keyValue: "TAOLACHUA",
                column: "MatKhau",
                value: "$2a$11$QZLza5JFper7Uml90H4KPOGVtgQwJYk3inBjAci.zIshvJaLg3WfC");
        }
    }
}
