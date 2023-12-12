using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagement_API.Migrations
{
    /// <inheritdoc />
    public partial class AddSupervisionListTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvailableSupervisionSlot",
                table: "SupervisionCohorts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SupervisionLists",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupervisorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DissertationCohortId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupervisionLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupervisionLists_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_SupervisionLists_AspNetUsers_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "SupervisionRequests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupervisorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DissertationCohortId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupervisionRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupervisionRequests_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_SupervisionRequests_AspNetUsers_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ea7f8af2-e6e2-4eae-b514-6a0a2a84ab74", "AQAAAAIAAYagAAAAEF4uddGqA5whjZLmi8trMx6uy9ht10OB9b10t9q0rEAhUH/RcsLHE7AwxWcNSqZAXQ==", "0b3f9b29-ff8a-42d4-a1de-c758f2a84e8d" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4ce24ee3-f1ff-4f57-98d4-34cccd674b9c", "AQAAAAIAAYagAAAAEM6pu5UyG3BreIUSiAG7iZEa17avVlRMxmmac5zN1r0v4x8+zQ8u+EfzWY7XJANusw==", "d6f4d9e1-8e4d-43fe-9a73-bb571564bb76" });

            migrationBuilder.CreateIndex(
                name: "IX_SupervisionLists_StudentId",
                table: "SupervisionLists",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_SupervisionLists_SupervisorId",
                table: "SupervisionLists",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_SupervisionRequests_StudentId",
                table: "SupervisionRequests",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_SupervisionRequests_SupervisorId",
                table: "SupervisionRequests",
                column: "SupervisorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupervisionLists");

            migrationBuilder.DropTable(
                name: "SupervisionRequests");

            migrationBuilder.DropColumn(
                name: "AvailableSupervisionSlot",
                table: "SupervisionCohorts");

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
        }
    }
}
