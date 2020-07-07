using Microsoft.EntityFrameworkCore.Migrations;

namespace PetFinderDAL.Migrations
{
    public partial class RemoveUnecessaryApplicationUserProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FavoriteList_ApplicationUserId",
                table: "FavoriteList");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteList_ApplicationUserId",
                table: "FavoriteList",
                column: "ApplicationUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FavoriteList_ApplicationUserId",
                table: "FavoriteList");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteList_ApplicationUserId",
                table: "FavoriteList",
                column: "ApplicationUserId",
                unique: true,
                filter: "[ApplicationUserId] IS NOT NULL");
        }
    }
}
