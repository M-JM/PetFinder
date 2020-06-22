using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PetFinderDAL.Migrations
{
    public partial class InitialDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Shelters",
                columns: table => new
                {
                    ShelterId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shelters", x => x.ShelterId);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    LocationtId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Street = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    HouseNumber = table.Column<int>(nullable: false),
                    Latitude = table.Column<int>(nullable: false),
                    Longitude = table.Column<int>(nullable: false),
                    ShelterId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.LocationtId);
                    table.ForeignKey(
                        name: "FK_Locations_Shelters_ShelterId",
                        column: x => x.ShelterId,
                        principalTable: "Shelters",
                        principalColumn: "ShelterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pets",
                columns: table => new
                {
                    PetId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Gender = table.Column<string>(nullable: true),
                    DOB = table.Column<DateTime>(nullable: false),
                    Size = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Social = table.Column<string>(nullable: true),
                    ShelterId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pets", x => x.PetId);
                    table.ForeignKey(
                        name: "FK_Pets_Shelters_ShelterId",
                        column: x => x.ShelterId,
                        principalTable: "Shelters",
                        principalColumn: "ShelterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PetColors",
                columns: table => new
                {
                    PetColorId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Color = table.Column<string>(nullable: true),
                    PetId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetColors", x => x.PetColorId);
                    table.ForeignKey(
                        name: "FK_PetColors_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "PetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PetPictures",
                columns: table => new
                {
                    PetPictureId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhotoPath = table.Column<string>(nullable: true),
                    PetId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetPictures", x => x.PetPictureId);
                    table.ForeignKey(
                        name: "FK_PetPictures_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "PetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PetRaces",
                columns: table => new
                {
                    PetRaceId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RaceName = table.Column<string>(nullable: true),
                    PetId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetRaces", x => x.PetRaceId);
                    table.ForeignKey(
                        name: "FK_PetRaces_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "PetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Locations_ShelterId",
                table: "Locations",
                column: "ShelterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PetColors_PetId",
                table: "PetColors",
                column: "PetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PetPictures_PetId",
                table: "PetPictures",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_PetRaces_PetId",
                table: "PetRaces",
                column: "PetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pets_ShelterId",
                table: "Pets",
                column: "ShelterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "PetColors");

            migrationBuilder.DropTable(
                name: "PetPictures");

            migrationBuilder.DropTable(
                name: "PetRaces");

            migrationBuilder.DropTable(
                name: "Pets");

            migrationBuilder.DropTable(
                name: "Shelters");
        }
    }
}
