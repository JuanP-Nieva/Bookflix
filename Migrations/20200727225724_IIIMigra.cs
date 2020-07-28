using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookflix.Migrations
{
    public partial class IIIMigra : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumeroComentario",
                table: "Reportes");

            migrationBuilder.AddColumn<int>(
                name: "PerfilId",
                table: "Reportes",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PerfilId",
                table: "Reportes");

            migrationBuilder.AddColumn<int>(
                name: "NumeroComentario",
                table: "Reportes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
