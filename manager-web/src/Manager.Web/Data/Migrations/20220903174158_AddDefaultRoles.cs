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
                values: new object[] { "02171634-f2c9-4054-966f-675702641552", "d4f5cdf6-9477-4b30-b011-1a2ef02c29f5", "superuser", "SUPERUSER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "9ffe1c8e-6dd6-4d1f-8e5a-93911e41cc90", "a123097a-63c2-4c22-be57-d08a7793c0b8", "user", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "bd2b7bc6-fa36-4988-9563-7ff609cf794c", "9e3856b0-725b-4dd5-85bf-78f5fbf9278c", "admin", "ADMIN" });
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
