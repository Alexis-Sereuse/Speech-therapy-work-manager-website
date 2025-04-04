using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionnaireOrthophonie.Migrations
{
    /// <inheritdoc />
    public partial class ColumnsNameModify : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Capacities",
                table: "Patients",
                newName: "Progress");

            migrationBuilder.RenameColumn(
                name: "AnamnesisInformation",
                table: "Patients",
                newName: "Anamnesis");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Progress",
                table: "Patients",
                newName: "Capacities");

            migrationBuilder.RenameColumn(
                name: "Anamnesis",
                table: "Patients",
                newName: "AnamnesisInformation");
        }
    }
}
