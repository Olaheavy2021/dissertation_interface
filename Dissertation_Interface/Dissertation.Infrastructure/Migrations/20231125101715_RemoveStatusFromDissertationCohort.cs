using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dissertation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStatusFromDissertationCohort : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.DropColumn(
                name: "Status",
                table: "DissertationCohorts");

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "DissertationCohorts",
                type: "int",
                nullable: false,
                defaultValue: 0);
    }
}