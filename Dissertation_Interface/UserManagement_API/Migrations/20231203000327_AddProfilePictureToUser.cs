using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagement_API.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class AddProfilePictureToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                values: new object[] { "38c994dc-4cb6-4f01-8057-9470f9a5b1e7", "AQAAAAIAAYagAAAAECqLTtzVA88qEvHtrlo+Ctx607DfpgpRCjyyu3ar/H4Y6PV3nqDNt0Wh2LK9kUkmWA==", null, "0bbddfb2-fef2-4c87-aa13-1747d5d6f749" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "ProfilePicture", "SecurityStamp" },
                values: new object[] { "f3118c6e-4bb3-46f1-b533-97cdaa20d370", "AQAAAAIAAYagAAAAEN76sZt0Fqp2RrX7qhnzc5nAu65r5xpJDkyXJgPiXyEBaiIzED2vmfdSADsFzqMfRA==", null, "c67fe57b-e877-4e6b-b523-1ad03a7997b1" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3038d4ef-c357-4e76-9d19-cd1e2ca92d03", "AQAAAAIAAYagAAAAEJdla+UFQSAn/dGbPcB/Vq96DbsKNYNnBEtKDwVrIaP0srPO30Ew6w1HZ3GWF4/Ifg==", "74aa2b69-b257-4ea2-a690-8f9653433d38" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "49431931-b504-44dd-adde-853d3aee4ab0", "AQAAAAIAAYagAAAAEAuFmbBXD4ylCp+BsLWPX+PQ0vHOlejSW6ysGb1mqEj+vWQsQFkP+Vmp2W6/nj1KNA==", "a29d8c8f-c511-4c49-bba2-95ed4980ce75" });
        }
    }
}