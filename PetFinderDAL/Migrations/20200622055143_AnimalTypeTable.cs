using Microsoft.EntityFrameworkCore.Migrations;

namespace PetFinderDAL.Migrations
{
    public partial class AnimalTypeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PetKindId",
                table: "Pets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PetKind",
                columns: table => new
                {
                    PetKindId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnimalType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetKind", x => x.PetKindId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pets_PetKindId",
                table: "Pets",
                column: "PetKindId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_PetKind_PetKindId",
                table: "Pets",
                column: "PetKindId",
                principalTable: "PetKind",
                principalColumn: "PetKindId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pets_PetKind_PetKindId",
                table: "Pets");

            migrationBuilder.DropTable(
                name: "PetKind");

            migrationBuilder.DropIndex(
                name: "IX_Pets_PetKindId",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "PetKindId",
                table: "Pets");
        }
    }
}
