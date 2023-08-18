using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdedonhaMVC.Migrations
{
    /// <inheritdoc />
    public partial class modifiqueioatributoInformacaodaentidadePalavraparaaceitarvaloresnulos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Informacao",
                table: "Palavra",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Informacao",
                table: "Palavra",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
