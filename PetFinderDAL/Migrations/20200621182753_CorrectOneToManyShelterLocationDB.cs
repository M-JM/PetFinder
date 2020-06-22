using Microsoft.EntityFrameworkCore.Migrations;

namespace PetFinderDAL.Migrations
{
    public partial class CorrectOneToManyShelterLocationDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Shelters_ShelterId",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_Locations_ShelterId",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "ShelterId",
                table: "Locations");

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Shelters",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Shelters_LocationId",
                table: "Shelters",
                column: "LocationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Shelters_Locations_LocationId",
                table: "Shelters",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "LocationtId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shelters_Locations_LocationId",
                table: "Shelters");

            migrationBuilder.DropIndex(
                name: "IX_Shelters_LocationId",
                table: "Shelters");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Shelters");

            migrationBuilder.AddColumn<int>(
                name: "ShelterId",
                table: "Locations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_ShelterId",
                table: "Locations",
                column: "ShelterId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_Shelters_ShelterId",
                table: "Locations",
                column: "ShelterId",
                principalTable: "Shelters",
                principalColumn: "ShelterId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
