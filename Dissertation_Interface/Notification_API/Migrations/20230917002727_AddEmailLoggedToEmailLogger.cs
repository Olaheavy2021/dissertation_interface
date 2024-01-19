using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notification_API.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class AddEmailLoggedToEmailLogger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.AddColumn<DateTime>(
                name: "EmailLogged",
                table: "EmailLoggers",
                type: "datetime2",
                nullable: true);

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropColumn(
                name: "EmailLogged",
                table: "EmailLoggers");
    }
}