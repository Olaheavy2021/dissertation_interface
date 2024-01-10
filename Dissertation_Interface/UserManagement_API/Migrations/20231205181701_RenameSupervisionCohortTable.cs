using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagement_API.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class RenameSupervisionCohortTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupervisionCohort_AspNetUsers_SupervisorId",
                table: "SupervisionCohort");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SupervisionCohort",
                table: "SupervisionCohort");

            migrationBuilder.RenameTable(
                name: "SupervisionCohort",
                newName: "SupervisionCohorts");

            migrationBuilder.RenameIndex(
                name: "IX_SupervisionCohort_SupervisorId",
                table: "SupervisionCohorts",
                newName: "IX_SupervisionCohorts_SupervisorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SupervisionCohorts",
                table: "SupervisionCohorts",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "15ba2b27-aa87-4f43-890d-c8d1e9845757", "AQAAAAIAAYagAAAAEJhjnT10xAVmdLP/QJUjIXaTlSonCQgokjtZcpqfSdQgPs0TCjVEy+gU3HqXQf5WJA==", "0dff038b-9064-49f6-8ebd-883e177c5ffd" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b574e39e-4c93-49e0-9521-0d1ecfe3a009", "AQAAAAIAAYagAAAAEJI5k/AT1uSnFkp0zOhLfyA/QZZJHJq/tnPunH5OAe8XJBOtRY79BMqvQRfGxA/hfQ==", "6dfedfa3-6f0f-4c77-a42f-ace207a05dd7" });

            migrationBuilder.AddForeignKey(
                name: "FK_SupervisionCohorts_AspNetUsers_SupervisorId",
                table: "SupervisionCohorts",
                column: "SupervisorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupervisionCohorts_AspNetUsers_SupervisorId",
                table: "SupervisionCohorts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SupervisionCohorts",
                table: "SupervisionCohorts");

            migrationBuilder.RenameTable(
                name: "SupervisionCohorts",
                newName: "SupervisionCohort");

            migrationBuilder.RenameIndex(
                name: "IX_SupervisionCohorts_SupervisorId",
                table: "SupervisionCohort",
                newName: "IX_SupervisionCohort_SupervisorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SupervisionCohort",
                table: "SupervisionCohort",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_SupervisionCohort_AspNetUsers_SupervisorId",
                table: "SupervisionCohort",
                column: "SupervisorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}