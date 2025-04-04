using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionnaireOrthophonie.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdInTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Sessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "PlanningOptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Patients",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PlanningOptions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Patients");
        }
    }
}
