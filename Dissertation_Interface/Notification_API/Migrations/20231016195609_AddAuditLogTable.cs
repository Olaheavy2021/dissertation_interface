using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notification_API.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class AddAuditLogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventTimeStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Outcome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetEntity = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(
                name: "AuditLogs");
    }
}