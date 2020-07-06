using Microsoft.EntityFrameworkCore.Migrations;

namespace PetFinderDAL.Migrations
{
    public partial class FavoriteListModificationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_FavoriteList_FavoritelistId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_FavoritelistId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "PetId",
                table: "FavoriteList",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FavoritelistId",
                table: "AspNetUsers",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteList_PetId",
                table: "FavoriteList",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_FavoritelistId",
                table: "AspNetUsers",
                column: "FavoritelistId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_FavoriteList_FavoritelistId",
                table: "AspNetUsers",
                column: "FavoritelistId",
                principalTable: "FavoriteList",
                principalColumn: "FavoritelistId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteList_Pets_PetId",
                table: "FavoriteList",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "PetId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_FavoriteList_FavoritelistId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteList_Pets_PetId",
                table: "FavoriteList");

            migrationBuilder.DropIndex(
                name: "IX_FavoriteList_PetId",
                table: "FavoriteList");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_FavoritelistId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PetId",
                table: "FavoriteList");

            migrationBuilder.AlterColumn<int>(
                name: "FavoritelistId",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_FavoritelistId",
                table: "AspNetUsers",
                column: "FavoritelistId",
                unique: true,
                filter: "[FavoritelistId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_FavoriteList_FavoritelistId",
                table: "AspNetUsers",
                column: "FavoritelistId",
                principalTable: "FavoriteList",
                principalColumn: "FavoritelistId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
