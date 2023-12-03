using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagement_API.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentAndCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CourseId",
                table: "AspNetUsers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DepartmentId",
                table: "AspNetUsers",
                type: "bigint",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "CourseId", "DepartmentId", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3038d4ef-c357-4e76-9d19-cd1e2ca92d03", null, null, "AQAAAAIAAYagAAAAEJdla+UFQSAn/dGbPcB/Vq96DbsKNYNnBEtKDwVrIaP0srPO30Ew6w1HZ3GWF4/Ifg==", "74aa2b69-b257-4ea2-a690-8f9653433d38" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "CourseId", "DepartmentId", "PasswordHash", "SecurityStamp" },
                values: new object[] { "49431931-b504-44dd-adde-853d3aee4ab0", null, null, "AQAAAAIAAYagAAAAEAuFmbBXD4ylCp+BsLWPX+PQ0vHOlejSW6ysGb1mqEj+vWQsQFkP+Vmp2W6/nj1KNA==", "a29d8c8f-c511-4c49-bba2-95ed4980ce75" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "481a2151-842a-42ff-ad7c-2d5a5d3dfa3e", "AQAAAAIAAYagAAAAEMUMeoTG71z6oKkb0MQ4ZUy3vCFMelHG276ZcuK21axLNwTperghOfbuB5URIyYnTg==", "1d0a8bdc-a7dc-438c-8708-672999786490" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "eb77afc6-41d4-4422-9b88-55c226a4f80c", "AQAAAAIAAYagAAAAEG5Vgn+Ld/LGbK2gi6tIttnuzm1ewzA8PjLlIpXuZHK12DPIUm2MBts0zgAFYy5xxw==", "d0668d66-dbb4-4a7b-a4f7-6c5719985dd8" });
        }
    }
}
