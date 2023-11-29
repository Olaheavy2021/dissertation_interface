using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dissertation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                table: "Supervisors");

            migrationBuilder.DropColumn(
                name: "StudentAllocation",
                table: "Supervisors");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "SupervisorInvites");

            migrationBuilder.AddColumn<long>(
                name: "DepartmentId",
                table: "Supervisors",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "DepartmentId",
                table: "SupervisorInvites",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Supervisors_DepartmentId",
                table: "Supervisors",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SupervisorInvites_DepartmentId",
                table: "SupervisorInvites",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupervisorInvites_Departments_DepartmentId",
                table: "SupervisorInvites",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Supervisors_Departments_DepartmentId",
                table: "Supervisors",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupervisorInvites_Departments_DepartmentId",
                table: "SupervisorInvites");

            migrationBuilder.DropForeignKey(
                name: "FK_Supervisors_Departments_DepartmentId",
                table: "Supervisors");

            migrationBuilder.DropIndex(
                name: "IX_Supervisors_DepartmentId",
                table: "Supervisors");

            migrationBuilder.DropIndex(
                name: "IX_SupervisorInvites_DepartmentId",
                table: "SupervisorInvites");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Supervisors");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "SupervisorInvites");

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "Supervisors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StudentAllocation",
                table: "Supervisors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "SupervisorInvites",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}