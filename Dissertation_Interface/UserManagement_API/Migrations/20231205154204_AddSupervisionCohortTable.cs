using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagement_API.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class AddSupervisionCohortTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupervisionCohort",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupervisorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DissertationCohortId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupervisionCohort", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupervisionCohort_AspNetUsers_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6c1e0cec-4321-41f8-a5dc-59f419947e8a", "AQAAAAIAAYagAAAAEHIiQMVT66vVf8Df+ArtRLcVENbssG4mjZE5NVndwsHXO5PMNE/VunyoQXYJuOXDgA==", "eb5c8cdc-2d09-438a-9287-5fd233f617d3" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b6e2b89c-dee2-4f26-895a-5460a5dc9852", "AQAAAAIAAYagAAAAEK0x06GLd8W/m/1HY8KpYZsV0S/MnQnQgHwWgFgSrlnXmz3CIOk3P9+vR0QrH1/AJg==", "b15c132f-6246-44e6-9d19-c83df9bec593" });

            migrationBuilder.CreateIndex(
                name: "IX_SupervisionCohort_SupervisorId",
                table: "SupervisionCohort",
                column: "SupervisorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupervisionCohort");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "38c994dc-4cb6-4f01-8057-9470f9a5b1e7", "AQAAAAIAAYagAAAAECqLTtzVA88qEvHtrlo+Ctx607DfpgpRCjyyu3ar/H4Y6PV3nqDNt0Wh2LK9kUkmWA==", "0bbddfb2-fef2-4c87-aa13-1747d5d6f749" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f3118c6e-4bb3-46f1-b533-97cdaa20d370", "AQAAAAIAAYagAAAAEN76sZt0Fqp2RrX7qhnzc5nAu65r5xpJDkyXJgPiXyEBaiIzED2vmfdSADsFzqMfRA==", "c67fe57b-e877-4e6b-b523-1ad03a7997b1" });
        }
    }
}