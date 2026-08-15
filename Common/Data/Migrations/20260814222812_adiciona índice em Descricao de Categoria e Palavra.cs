using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdedonhaMVC.Common.Data.Migrations
{
    /// <inheritdoc />
    public partial class adicionaíndiceemDescricaodeCategoriaePalavra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Palavra",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Categoria",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Palavra_Descricao",
                table: "Palavra",
                column: "Descricao");

            migrationBuilder.CreateIndex(
                name: "IX_Categoria_Descricao",
                table: "Categoria",
                column: "Descricao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Palavra_Descricao",
                table: "Palavra");

            migrationBuilder.DropIndex(
                name: "IX_Categoria_Descricao",
                table: "Categoria");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Palavra",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Categoria",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
