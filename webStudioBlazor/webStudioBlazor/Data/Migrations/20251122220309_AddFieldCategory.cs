using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webStudioBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CategoryId",
                table: "Reviews",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Categories_CategoryId",
                table: "Reviews",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Categories_CategoryId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_CategoryId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Reviews");
        }
    }
}
