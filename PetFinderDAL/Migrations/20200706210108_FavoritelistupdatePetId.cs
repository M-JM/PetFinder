using Microsoft.EntityFrameworkCore.Migrations;

namespace PetFinderDAL.Migrations
{
    public partial class FavoritelistupdatePetId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteList_Pets_PetId",
                table: "FavoriteList");

            migrationBuilder.AlterColumn<int>(
                name: "PetId",
                table: "FavoriteList",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteList_Pets_PetId",
                table: "FavoriteList",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "PetId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteList_Pets_PetId",
                table: "FavoriteList");

            migrationBuilder.AlterColumn<int>(
                name: "PetId",
                table: "FavoriteList",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteList_Pets_PetId",
                table: "FavoriteList",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "PetId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
