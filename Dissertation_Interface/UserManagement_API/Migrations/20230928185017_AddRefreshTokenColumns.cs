using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagement_API.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class AddRefreshTokenColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "RefreshToken", "RefreshTokenExpiryTime", "SecurityStamp" },
                values: new object[] { "f7ade31c-9336-4222-87f2-da5aff786ddf", "AQAAAAIAAYagAAAAEL6cSHnS/Re6jrf5XnuqztvwOmOV0QtqesOhpdowbHRSzNoCuK+6Jvd8CvFdYE3qQQ==", "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "eba40ad9-cc8d-47af-9388-a43c2bcf2f4a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "RefreshToken", "RefreshTokenExpiryTime", "SecurityStamp" },
                values: new object[] { "16b4c1f9-a5ab-4c69-8033-1254ee490556", "AQAAAAIAAYagAAAAEBOTxnoSIzcXBYTSZ9eRparqHt+dnLBDFX3GqkPNYDB8Bwp/MEY/GZVs2D6SBz5hxw==", "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "7050bb5a-563b-4748-be89-06dec440e634" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a815e166-94a8-4381-bfb7-f5b19dc9200b", "AQAAAAIAAYagAAAAENwMO/66+NOOjZrfbVqva9nWZ4TO+SA5HCZyyk5VvflJGOMnKAL0Q01DTYrncuvF5g==", "d3dca18e-5e7d-44bf-ac3a-565f4a65a2cb" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1037e3b1-62f0-49f0-95a1-85b6157758fc", "AQAAAAIAAYagAAAAEFbazlstcB9oEag17TGd1QhWB8Yb4P2hfb4xDg/O7NNoIS/AC61mmmNG2N8a8uRgyw==", "8dc7a998-339f-4894-9938-d08e4d0f577c" });
        }
    }
}