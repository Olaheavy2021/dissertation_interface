using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagement_API.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class AddSupervisionSlotToSupervisionCohort : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SupervisionSlot",
                table: "SupervisionCohort",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a9cc3394-e966-4f1b-bb8b-6c39fe8012f3", "AQAAAAIAAYagAAAAEKpqrFhph2w19hNQPGdmWDSJwLm+wh9E4eDcK0fpwpI6gQDYpDfrPYV7fFWMXjtmJw==", "36e67c94-247e-4907-8460-676a53455b0a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fcf6688d-502d-4253-8d32-8c78ad717546", "AQAAAAIAAYagAAAAEKiNfPqhMAtmZyNzY/xzwzSEF6h0/3MIWRFzNnp9qHVrVzz/kqmLth15GtjaCtG0FA==", "05f89288-ded2-4134-aaa7-53257f973237" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupervisionSlot",
                table: "SupervisionCohort");

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
        }
    }
}