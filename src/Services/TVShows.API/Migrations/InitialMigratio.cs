namespace AUTOPAL.RTL.TVMaze.Services.TVShows.API.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class InitialMigratio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation(name: "SqlServer:ValueGenerationStrategy",
                            value: SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    BirthDate = table.Column<DateTime>(nullable: false),
                    TVMazeId = table.Column<int>(nullable: false)
                },
                constraints: table => { table.PrimaryKey(name: "PK_Person", columns: x => x.Id); });

            migrationBuilder.CreateTable(
                name: "Show",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation(name: "SqlServer:ValueGenerationStrategy",
                            value: SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    TVMazeId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<long>(nullable: false)
                },
                constraints: table => { table.PrimaryKey(name: "PK_Show", columns: x => x.Id); });

            migrationBuilder.CreateTable(
                name: "ShowPerson",
                columns: table => new
                {
                    ShowId = table.Column<int>(nullable: false),
                    PersonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(name: "PK_ShowPerson", columns: x => new {x.PersonId, x.ShowId});
                    table.ForeignKey(
                        name: "FK_ShowPerson_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShowPerson_Show_ShowId",
                        column: x => x.ShowId,
                        principalTable: "Show",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShowPerson_ShowId",
                table: "ShowPerson",
                column: "ShowId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShowPerson");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropTable(
                name: "Show");
        }
    }
}