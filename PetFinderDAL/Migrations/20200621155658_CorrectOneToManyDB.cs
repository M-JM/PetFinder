using Microsoft.EntityFrameworkCore.Migrations;

namespace PetFinderDAL.Migrations
{
    public partial class CorrectOneToManyDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PetColors_Pets_PetId",
                table: "PetColors");

            migrationBuilder.DropForeignKey(
                name: "FK_PetRaces_Pets_PetId",
                table: "PetRaces");

            migrationBuilder.DropIndex(
                name: "IX_PetRaces_PetId",
                table: "PetRaces");

            migrationBuilder.DropIndex(
                name: "IX_PetColors_PetId",
                table: "PetColors");

            migrationBuilder.DropColumn(
                name: "PetId",
                table: "PetRaces");

            migrationBuilder.DropColumn(
                name: "PetId",
                table: "PetColors");

            migrationBuilder.AddColumn<int>(
                name: "PetColorId",
                table: "Pets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PetRaceId",
                table: "Pets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Pets_PetColorId",
                table: "Pets",
                column: "PetColorId");

            migrationBuilder.CreateIndex(
                name: "IX_Pets_PetRaceId",
                table: "Pets",
                column: "PetRaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_PetColors_PetColorId",
                table: "Pets",
                column: "PetColorId",
                principalTable: "PetColors",
                principalColumn: "PetColorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_PetRaces_PetRaceId",
                table: "Pets",
                column: "PetRaceId",
                principalTable: "PetRaces",
                principalColumn: "PetRaceId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pets_PetColors_PetColorId",
                table: "Pets");

            migrationBuilder.DropForeignKey(
                name: "FK_Pets_PetRaces_PetRaceId",
                table: "Pets");

            migrationBuilder.DropIndex(
                name: "IX_Pets_PetColorId",
                table: "Pets");

            migrationBuilder.DropIndex(
                name: "IX_Pets_PetRaceId",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "PetColorId",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "PetRaceId",
                table: "Pets");

            migrationBuilder.AddColumn<int>(
                name: "PetId",
                table: "PetRaces",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PetId",
                table: "PetColors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PetRaces_PetId",
                table: "PetRaces",
                column: "PetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PetColors_PetId",
                table: "PetColors",
                column: "PetId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PetColors_Pets_PetId",
                table: "PetColors",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "PetId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PetRaces_Pets_PetId",
                table: "PetRaces",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "PetId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
