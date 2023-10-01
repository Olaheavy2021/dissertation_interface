using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagement_API.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminLockedOutFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLockedOutByAdmin",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "IsLockedOutByAdmin", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8b35d644-0a0a-41b4-86d3-57ed934dd422", false, "AQAAAAIAAYagAAAAEJdSUvMo0cE9OT8V5ABS/VQFWouD3vKy/6BnaOawRd84OT5Wk3SAdLgxfHZQy/gWCQ==", "76cb3b53-8a42-4849-9c0f-6aa53eef1a18" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "IsLockedOutByAdmin", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7dec5759-4ab9-450b-abb2-5abce76004d3", false, "AQAAAAIAAYagAAAAEJehsZNZfEcc8RNLkjHxCBS/CtPSl6gVTI/EfhoEbu9W90USN9X5Fis8vB6GgE2TIg==", "20b74ef9-9bbf-4b72-a980-14ee089eb21c" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLockedOutByAdmin",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f7ade31c-9336-4222-87f2-da5aff786ddf", "AQAAAAIAAYagAAAAEL6cSHnS/Re6jrf5XnuqztvwOmOV0QtqesOhpdowbHRSzNoCuK+6Jvd8CvFdYE3qQQ==", "eba40ad9-cc8d-47af-9388-a43c2bcf2f4a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "16b4c1f9-a5ab-4c69-8033-1254ee490556", "AQAAAAIAAYagAAAAEBOTxnoSIzcXBYTSZ9eRparqHt+dnLBDFX3GqkPNYDB8Bwp/MEY/GZVs2D6SBz5hxw==", "7050bb5a-563b-4748-be89-06dec440e634" });
        }
    }
}