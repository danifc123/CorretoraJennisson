using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CorretoraJenissonLuckwuAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddComodosToImovel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Banheiros",
                table: "Imoveis",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Cozinhas",
                table: "Imoveis",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GaragemCoberta",
                table: "Imoveis",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GaragemDescoberta",
                table: "Imoveis",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Lavabos",
                table: "Imoveis",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Salas",
                table: "Imoveis",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Suites",
                table: "Imoveis",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Banheiros",
                table: "Imoveis");

            migrationBuilder.DropColumn(
                name: "Cozinhas",
                table: "Imoveis");

            migrationBuilder.DropColumn(
                name: "GaragemCoberta",
                table: "Imoveis");

            migrationBuilder.DropColumn(
                name: "GaragemDescoberta",
                table: "Imoveis");

            migrationBuilder.DropColumn(
                name: "Lavabos",
                table: "Imoveis");

            migrationBuilder.DropColumn(
                name: "Salas",
                table: "Imoveis");

            migrationBuilder.DropColumn(
                name: "Suites",
                table: "Imoveis");
        }
    }
}
