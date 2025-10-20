using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webStudioBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddClientOrdersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Appointments_AppointmentId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_AppointmentId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "AppointmentId",
                table: "Orders",
                newName: "ClientId");

            migrationBuilder.CreateTable(
                name: "ClientOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientFirstName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ClientLastName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ClientPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "date", nullable: false),
                    City = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    AddressNewPostOffice = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientOrders_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientOrders_OrderId",
                table: "ClientOrders",
                column: "OrderId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientOrders");

            migrationBuilder.RenameColumn(
                name: "ClientId",
                table: "Orders",
                newName: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AppointmentId",
                table: "Orders",
                column: "AppointmentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Appointments_AppointmentId",
                table: "Orders",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
