using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delopro.Data.Migrations.PostgresDeloproDb
{
    /// <inheritdoc />
    public partial class DefinedDefaultValuesForUserIdandIsReadinMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Messages",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<bool>(
                name: "IsRead",
                table: "Messages",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Messages",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<bool>(
                name: "IsRead",
                table: "Messages",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);
        }
    }
}
