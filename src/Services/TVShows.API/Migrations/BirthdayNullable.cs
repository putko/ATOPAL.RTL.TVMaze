namespace AUTOPAL.RTL.TVMaze.Services.TVShows.API.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class BirthdayNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "BirthDate",
                table: "Person",
                nullable: true,
                oldClrType: typeof(DateTime));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "BirthDate",
                table: "Person",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}