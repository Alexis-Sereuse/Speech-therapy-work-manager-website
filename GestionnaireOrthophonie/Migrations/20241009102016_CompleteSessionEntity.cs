using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionnaireOrthophonie.Migrations
{
    /// <inheritdoc />
    public partial class CompleteSessionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Sessions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Sessions");
        }
    }
}
