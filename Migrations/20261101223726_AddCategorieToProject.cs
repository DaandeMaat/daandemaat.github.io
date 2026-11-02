using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace daandemaat.github.io.Migrations
{
    /// <inheritdoc />
    public partial class AddCategorieToProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Categorie",
                table: "Projecten",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categorie",
                table: "Projecten");
        }
    }
}
