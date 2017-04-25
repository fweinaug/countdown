using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CountdownApp.Migrations
{
  public partial class FirstMigration : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: "Countdowns",
        columns: table => new
        {
          Id = table.Column<int>(nullable: false)
            .Annotation("Sqlite:Autoincrement", true),
          Created = table.Column<DateTime>(nullable: false),
          Date = table.Column<DateTime>(nullable: false),
          FinishedNotification = table.Column<bool>(nullable: false),
          Guid = table.Column<string>(nullable: true),
          HasImage = table.Column<bool>(nullable: false),
          ImageData = table.Column<byte[]>(nullable: true),
          IsRecurrent = table.Column<bool>(nullable: false),
          Name = table.Column<string>(nullable: true),
          PinnedToStart = table.Column<bool>(nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_Countdowns", x => x.Id);
        });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
        name: "Countdowns");
    }
  }
}