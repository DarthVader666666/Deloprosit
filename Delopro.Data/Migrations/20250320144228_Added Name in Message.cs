﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delopro.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedNameinMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Messages");
        }
    }
}
