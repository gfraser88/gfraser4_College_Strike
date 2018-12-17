using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace gfraser4_College_Strike.Data.MOMigrations
{
    public partial class images : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "imageContent",
                schema: "CS",
                table: "Members",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "imageFileName",
                schema: "CS",
                table: "Members",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "imageMimeType",
                schema: "CS",
                table: "Members",
                maxLength: 256,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "imageContent",
                schema: "CS",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "imageFileName",
                schema: "CS",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "imageMimeType",
                schema: "CS",
                table: "Members");
        }
    }
}
