using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notification_API.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailIdentifierToEmailLogger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailIdentifier",
                table: "EmailLoggers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLoggers_EmailIdentifier",
                table: "EmailLoggers",
                column: "EmailIdentifier",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmailLoggers_EmailIdentifier",
                table: "EmailLoggers");

            migrationBuilder.DropColumn(
                name: "EmailIdentifier",
                table: "EmailLoggers");
        }
    }
}