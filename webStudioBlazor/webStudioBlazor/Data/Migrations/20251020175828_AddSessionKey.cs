using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webStudioBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SessionKey",
                table: "Orders",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SessionKey",
                table: "Orders",
                column: "SessionKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_SessionKey",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SessionKey",
                table: "Orders");
        }
    }
}
