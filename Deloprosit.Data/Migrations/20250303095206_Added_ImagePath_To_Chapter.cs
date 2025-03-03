using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Deloprosit.Data.Migrations
{
    /// <inheritdoc />
    public partial class Added_ImagePath_To_Chapter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Chapters",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Chapters");
        }
    }
}
