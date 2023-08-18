using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdedonhaMVC.Migrations
{
    /// <inheritdoc />
    public partial class remoçãodoatributoimagemdatabelaPalavra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Imagem",
                table: "Palavra");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Imagem",
                table: "Palavra",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
