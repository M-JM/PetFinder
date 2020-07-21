using Microsoft.EntityFrameworkCore.Migrations;

namespace PetFinderDAL.Migrations
{
    public partial class AddedColorPropToStatusTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pets_Shelters_ShelterId",
                table: "Pets");

            migrationBuilder.AlterColumn<int>(
                name: "ShelterId",
                table: "Pets",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Pets",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Pets",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Pets",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "AppointmentStatuses",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_Shelters_ShelterId",
                table: "Pets",
                column: "ShelterId",
                principalTable: "Shelters",
                principalColumn: "ShelterId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pets_Shelters_ShelterId",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "AppointmentStatuses");

            migrationBuilder.AlterColumn<int>(
                name: "ShelterId",
                table: "Pets",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Pets",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Pets",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Pets",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_Shelters_ShelterId",
                table: "Pets",
                column: "ShelterId",
                principalTable: "Shelters",
                principalColumn: "ShelterId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
