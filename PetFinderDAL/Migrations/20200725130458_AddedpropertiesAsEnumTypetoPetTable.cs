using Microsoft.EntityFrameworkCore.Migrations;

namespace PetFinderDAL.Migrations
{
    public partial class AddedpropertiesAsEnumTypetoPetTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Social",
                table: "Pets");

            migrationBuilder.AddColumn<int>(
                name: "Appartmentfit",
                table: "Pets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "KidsFriendly",
                table: "Pets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SocialWithCats",
                table: "Pets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SocialWithDogs",
                table: "Pets",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Appartmentfit",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "KidsFriendly",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "SocialWithCats",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "SocialWithDogs",
                table: "Pets");

            migrationBuilder.AddColumn<string>(
                name: "Social",
                table: "Pets",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
