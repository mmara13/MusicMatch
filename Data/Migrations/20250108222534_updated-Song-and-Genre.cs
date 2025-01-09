using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicMatch.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatedSongandGenre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Genre",
                table: "Songs");

            migrationBuilder.AddColumn<int>(
                name: "GenreId",
                table: "Songs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Songs_GenreId",
                table: "Songs",
                column: "GenreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_Genres_GenreId",
                table: "Songs",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Songs_Genres_GenreId",
                table: "Songs");

            migrationBuilder.DropIndex(
                name: "IX_Songs_GenreId",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "GenreId",
                table: "Songs");

            migrationBuilder.AddColumn<string>(
                name: "Genre",
                table: "Songs",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
