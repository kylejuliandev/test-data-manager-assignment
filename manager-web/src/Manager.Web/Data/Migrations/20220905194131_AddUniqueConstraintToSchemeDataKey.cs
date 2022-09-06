using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Manager.Web.Data.Migrations
{
    public partial class AddUniqueConstraintToSchemeDataKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SchemeData_Key",
                table: "SchemeData",
                column: "Key",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SchemeData_Key",
                table: "SchemeData");
        }
    }
}
