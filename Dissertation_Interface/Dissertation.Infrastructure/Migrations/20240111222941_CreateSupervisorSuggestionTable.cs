using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dissertation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateSupervisorSuggestionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.CreateTable(
                name: "SupervisorSuggestions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AvailableSlot = table.Column<int>(type: "int", nullable: false),
                    CompatibilityScore = table.Column<double>(type: "float", nullable: false),
                    ResearchArea = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SupervisorId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudentTopic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudentId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupervisorSuggestions", x => x.Id);
                });

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(
                name: "SupervisorSuggestions");
    }
}