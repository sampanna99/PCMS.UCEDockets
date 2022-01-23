using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UCEDockets.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dockets",
                columns: table => new
                {
                    DocketID = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    XMLDocket = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    District = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    County = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Filed = table.Column<DateTime>(type: "datetime2", nullable: true)
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
