using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CommonAuthApp.API.Migrations
{
    /// <inheritdoc />
    public partial class schoolSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SchoolMenus",
                columns: table => new
                {
                    MenuId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MenuSerial = table.Column<int>(type: "integer", nullable: false),
                    MenuName = table.Column<string>(type: "text", nullable: true),
                    MenuUrlPath = table.Column<string>(type: "text", nullable: true),
                    MenuIsActive = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolMenus", x => x.MenuId);
                });

            migrationBuilder.CreateTable(
                name: "Schools",
                columns: table => new
                {
                    SchoolId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SchoolCode = table.Column<string>(type: "text", nullable: true),
                    SchoolName = table.Column<string>(type: "text", nullable: true),
                    SchoolEiin = table.Column<string>(type: "text", nullable: true),
                    SchoolType = table.Column<string>(type: "text", nullable: true),
                    SchoolDivision = table.Column<string>(type: "text", nullable: true),
                    SchoolDistrict = table.Column<string>(type: "text", nullable: true),
                    SchoolThana = table.Column<string>(type: "text", nullable: true),
                    SchoolPostalCode = table.Column<string>(type: "text", nullable: true),
                    SchoolArea = table.Column<string>(type: "text", nullable: true),
                    SchoolAddress = table.Column<string>(type: "text", nullable: true),
                    SchoolContactPerson = table.Column<string>(type: "text", nullable: true),
                    SchoolContactPersonPosition = table.Column<string>(type: "text", nullable: true),
                    SchoolConactPersonMobileNo = table.Column<string>(type: "text", nullable: true),
                    SchoolStatus = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SchoolLogoPath = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schools", x => x.SchoolId);
                });

            migrationBuilder.CreateTable(
                name: "SchoolSystemDetails",
                columns: table => new
                {
                    SystemDetailsId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    SchoolServerIP = table.Column<string>(type: "text", nullable: true),
                    SchoolServerPort = table.Column<int>(type: "integer", nullable: false),
                    SchoolDBName = table.Column<string>(type: "text", nullable: true),
                    SchoolDBServer = table.Column<string>(type: "text", nullable: true),
                    SchoolDBPort = table.Column<int>(type: "integer", nullable: false),
                    SchoolDBUsername = table.Column<string>(type: "text", nullable: true),
                    SchoolDBPass = table.Column<string>(type: "text", nullable: true),
                    SchoolDBSystemName = table.Column<string>(type: "text", nullable: true),
                    SchoolDBConnectionString = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolSystemDetails", x => x.SystemDetailsId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SchoolMenus");

            migrationBuilder.DropTable(
                name: "Schools");

            migrationBuilder.DropTable(
                name: "SchoolSystemDetails");
        }
    }
}
