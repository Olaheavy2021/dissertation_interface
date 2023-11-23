using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagement_API.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedDatetoUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "CreatedOn", "PasswordHash", "SecurityStamp" },
                values: new object[] { "481a2151-842a-42ff-ad7c-2d5a5d3dfa3e", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AQAAAAIAAYagAAAAEMUMeoTG71z6oKkb0MQ4ZUy3vCFMelHG276ZcuK21axLNwTperghOfbuB5URIyYnTg==", "1d0a8bdc-a7dc-438c-8708-672999786490" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "CreatedOn", "PasswordHash", "SecurityStamp" },
                values: new object[] { "eb77afc6-41d4-4422-9b88-55c226a4f80c", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AQAAAAIAAYagAAAAEG5Vgn+Ld/LGbK2gi6tIttnuzm1ewzA8PjLlIpXuZHK12DPIUm2MBts0zgAFYy5xxw==", "d0668d66-dbb4-4a7b-a4f7-6c5719985dd8" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8b35d644-0a0a-41b4-86d3-57ed934dd422", "AQAAAAIAAYagAAAAEJdSUvMo0cE9OT8V5ABS/VQFWouD3vKy/6BnaOawRd84OT5Wk3SAdLgxfHZQy/gWCQ==", "76cb3b53-8a42-4849-9c0f-6aa53eef1a18" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7dec5759-4ab9-450b-abb2-5abce76004d3", "AQAAAAIAAYagAAAAEJehsZNZfEcc8RNLkjHxCBS/CtPSl6gVTI/EfhoEbu9W90USN9X5Fis8vB6GgE2TIg==", "20b74ef9-9bbf-4b72-a980-14ee089eb21c" });
        }
    }
}
