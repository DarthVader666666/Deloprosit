using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Deloprosit.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdminandOwnerInitialCredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Avatar", "BirthDate", "City", "Country", "Email", "FirstName", "Info", "IsConfirmed", "LastName", "Nickname", "Password", "RegisterDate", "UserTitle" },
                values: new object[,]
                {
                    { 1, null, null, null, null, "mHdyjukniQqXvq8Ulori1g==", null, null, true, null, "alex", "eK+Th1R1aYQxoYblzPPL8w==", null, null },
                    { 2, null, null, null, null, "JtfP1IxKgKVGB4ADFXFnvA==", null, null, true, null, "admin", "efavXKTzRTFnR7w69A7OJA==", null, null }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2);
        }
    }
}
