using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagement_API.Migrations
{
    /// <inheritdoc />
    public partial class AddProfilePictureTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "ProfilePictures",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfilePictures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfilePictures_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "41f40866-1a02-420c-b924-7d0c055b8ca5", "AQAAAAIAAYagAAAAEA4pDy32+LRY89ubjfcGnSlMvrU/So4CvcSvOX5E0fSwIBPX1mtVRFHeayRnti311A==", "bef2a490-845a-4f09-b477-92b856d0a7cb" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fe802224-b4fc-4bb4-968f-c8862504a072", "AQAAAAIAAYagAAAAEJsNV2ZohJxU7ZKFzTBp8SwewCBd+TAytM7TAJpDe34oLjiFq0F1SLtpCDC+7eR4Bw==", "b5b79c1c-256f-470c-8637-8d83a2991112" });

            migrationBuilder.CreateIndex(
                name: "IX_ProfilePictures_UserId",
                table: "ProfilePictures",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfilePictures");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "ProfilePicture", "SecurityStamp" },
                values: new object[] { "ea7f8af2-e6e2-4eae-b514-6a0a2a84ab74", "AQAAAAIAAYagAAAAEF4uddGqA5whjZLmi8trMx6uy9ht10OB9b10t9q0rEAhUH/RcsLHE7AwxWcNSqZAXQ==", null, "0b3f9b29-ff8a-42d4-a1de-c758f2a84e8d" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "ProfilePicture", "SecurityStamp" },
                values: new object[] { "4ce24ee3-f1ff-4f57-98d4-34cccd674b9c", "AQAAAAIAAYagAAAAEM6pu5UyG3BreIUSiAG7iZEa17avVlRMxmmac5zN1r0v4x8+zQ8u+EfzWY7XJANusw==", null, "d6f4d9e1-8e4d-43fe-9a73-bb571564bb76" });
        }
    }
}