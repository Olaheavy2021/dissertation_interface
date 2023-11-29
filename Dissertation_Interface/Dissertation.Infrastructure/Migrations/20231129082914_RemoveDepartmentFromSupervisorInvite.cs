using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dissertation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDepartmentFromSupervisorInvite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupervisorInvites_Departments_DepartmentId",
                table: "SupervisorInvites");

            migrationBuilder.DropIndex(
                name: "IX_SupervisorInvites_DepartmentId",
                table: "SupervisorInvites");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "SupervisorInvites");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DepartmentId",
                table: "SupervisorInvites",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

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
        }
    }
}
