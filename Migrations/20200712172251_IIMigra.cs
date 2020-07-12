using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookflix.Migrations
{
    public partial class IIMigra : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Perfil_Comenta_Libros",
                table: "Perfil_Comenta_Libros");

            migrationBuilder.DropIndex(
                name: "IX_Perfil_Comenta_Libros_LibroId",
                table: "Perfil_Comenta_Libros");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Perfil_Comenta_Libros");

            migrationBuilder.AddColumn<int>(
                name: "NumeroComentario",
                table: "Perfil_Comenta_Libros",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Perfil_Comenta_Libros",
                table: "Perfil_Comenta_Libros",
                columns: new[] { "LibroId", "NumeroComentario" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Perfil_Comenta_Libros",
                table: "Perfil_Comenta_Libros");

            migrationBuilder.DropColumn(
                name: "NumeroComentario",
                table: "Perfil_Comenta_Libros");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Perfil_Comenta_Libros",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Perfil_Comenta_Libros",
                table: "Perfil_Comenta_Libros",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Perfil_Comenta_Libros_LibroId",
                table: "Perfil_Comenta_Libros",
                column: "LibroId");
        }
    }
}
