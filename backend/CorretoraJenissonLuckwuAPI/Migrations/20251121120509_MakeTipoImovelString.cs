using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CorretoraJenissonLuckwuAPI.Migrations
{
    /// <inheritdoc />
    public partial class MakeTipoImovelString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TipoImovelTemp",
                table: "Imoveis",
                type: "text",
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.Sql(
                @"UPDATE ""Imoveis"" 
                  SET ""TipoImovelTemp"" = CASE ""TipoImovel""
                      WHEN 0 THEN 'Casa'
                      WHEN 1 THEN 'Apartamento'
                      WHEN 2 THEN 'Terreno'
                      ELSE 'Outro'
                  END");

            migrationBuilder.DropColumn(
                name: "TipoImovel",
                table: "Imoveis");

            migrationBuilder.RenameColumn(
                name: "TipoImovelTemp",
                table: "Imoveis",
                newName: "TipoImovel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TipoImovelTemp",
                table: "Imoveis",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                @"UPDATE ""Imoveis"" 
                  SET ""TipoImovelTemp"" = CASE 
                      WHEN UPPER(""TipoImovel"") = 'CASA' THEN 0
                      WHEN UPPER(""TipoImovel"") = 'APARTAMENTO' THEN 1
                      WHEN UPPER(""TipoImovel"") = 'TERRENO' THEN 2
                      ELSE 0
                  END");

            migrationBuilder.DropColumn(
                name: "TipoImovel",
                table: "Imoveis");

            migrationBuilder.RenameColumn(
                name: "TipoImovelTemp",
                table: "Imoveis",
                newName: "TipoImovel");
        }
    }
}
