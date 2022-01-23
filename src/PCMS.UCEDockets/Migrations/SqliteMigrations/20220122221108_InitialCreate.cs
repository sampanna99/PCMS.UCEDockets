using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCMS.UCEDockets.Migrations.SqliteMigrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dockets",
                columns: table => new
                {
                    DocketID = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    XMLDocket = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    District = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    County = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    Filed = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dockets", x => x.DocketID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dockets");
        }
    }
}
