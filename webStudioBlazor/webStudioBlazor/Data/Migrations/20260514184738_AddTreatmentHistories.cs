using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webStudioBlazor.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTreatmentHistories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TreatmentHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    VisitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcedureName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProcedureDescription = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    SkinBeforeDescription = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    SkinAfterDescription = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Recommendations = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    MasterComment = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TreatmentPhotos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TreatmentHistoryId = table.Column<int>(type: "int", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    PhotoType = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreatmentPhotos_TreatmentHistories_TreatmentHistoryId",
                        column: x => x.TreatmentHistoryId,
                        principalTable: "TreatmentHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentHistories_ClientId",
                table: "TreatmentHistories",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentHistories_ClientId_VisitDate",
                table: "TreatmentHistories",
                columns: new[] { "ClientId", "VisitDate" });

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentPhotos_TreatmentHistoryId",
                table: "TreatmentPhotos",
                column: "TreatmentHistoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TreatmentPhotos");

            migrationBuilder.DropTable(
                name: "TreatmentHistories");
        }
    }
}
