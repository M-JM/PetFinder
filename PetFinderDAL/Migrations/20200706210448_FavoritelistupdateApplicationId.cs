using Microsoft.EntityFrameworkCore.Migrations;

namespace PetFinderDAL.Migrations
{
    public partial class FavoritelistupdateApplicationId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_FavoriteList_FavoritelistId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_FavoritelistId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FavoritelistId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "FavoriteList",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteList_ApplicationUserId",
                table: "FavoriteList",
                column: "ApplicationUserId",
                unique: true,
                filter: "[ApplicationUserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteList_AspNetUsers_ApplicationUserId",
                table: "FavoriteList",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteList_AspNetUsers_ApplicationUserId",
                table: "FavoriteList");

            migrationBuilder.DropIndex(
                name: "IX_FavoriteList_ApplicationUserId",
                table: "FavoriteList");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "FavoriteList");

            migrationBuilder.AddColumn<int>(
                name: "FavoritelistId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
        }
    }
}
