using Microsoft.EntityFrameworkCore.Migrations;

namespace PetFinderDAL.Migrations
{
    public partial class Favoritelist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FavoritelistId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FavoriteList",
                columns: table => new
                {
                    FavoritelistId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteList", x => x.FavoritelistId);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_FavoriteList_FavoritelistId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "FavoriteList");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_FavoritelistId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FavoritelistId",
                table: "AspNetUsers");
        }
    }
}
