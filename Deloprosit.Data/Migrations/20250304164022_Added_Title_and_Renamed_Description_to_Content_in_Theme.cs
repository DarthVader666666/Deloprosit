using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Deloprosit.Data.Migrations
{
    /// <inheritdoc />
    public partial class Added_Title_and_Renamed_Description_to_Content_in_Theme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Themes");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Themes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThemeTitle",
                table: "Themes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "Themes");

            migrationBuilder.DropColumn(
                name: "ThemeTitle",
                table: "Themes");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Themes",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");
        }
    }
}
