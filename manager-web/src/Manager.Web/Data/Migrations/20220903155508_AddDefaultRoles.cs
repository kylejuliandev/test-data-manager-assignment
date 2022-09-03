using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Manager.Web.Data.Migrations
{
    public partial class AddDefaultRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "02171634-f2c9-4054-966f-675702641552", "9458c513-5553-46bd-8f63-d23b687d3b59", "superadmin", "SUPERADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "9ffe1c8e-6dd6-4d1f-8e5a-93911e41cc90", "c408595a-4422-43bc-aeed-53c31b9aba59", "user", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "bd2b7bc6-fa36-4988-9563-7ff609cf794c", "f8c8ab7d-6604-4eb0-9c87-5b44baaf17b0", "admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "02171634-f2c9-4054-966f-675702641552");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9ffe1c8e-6dd6-4d1f-8e5a-93911e41cc90");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bd2b7bc6-fa36-4988-9563-7ff609cf794c");
        }
    }
}
