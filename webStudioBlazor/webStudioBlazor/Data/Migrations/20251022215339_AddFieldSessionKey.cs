using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webStudioBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldSessionKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderItems_OrderId_TherapyId",
                table: "OrderItems");

            migrationBuilder.AlterColumn<int>(
                name: "TherapyId",
                table: "OrderItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "SessionKey",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId_TherapyId",
                table: "OrderItems",
                columns: new[] { "OrderId", "TherapyId" },
                unique: true,
                filter: "[TherapyId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderItems_OrderId_TherapyId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "SessionKey",
                table: "OrderItems");

            migrationBuilder.AlterColumn<int>(
                name: "TherapyId",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId_TherapyId",
                table: "OrderItems",
                columns: new[] { "OrderId", "TherapyId" },
                unique: true);
        }
    }
}
