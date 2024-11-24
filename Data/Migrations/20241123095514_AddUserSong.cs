using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicMatch.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistSongs_AspNetUsers_AddedBy",
                table: "PlaylistSongs");

            migrationBuilder.RenameColumn(
                name: "AddedBy",
                table: "PlaylistSongs",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PlaylistSongs_AddedBy",
                table: "PlaylistSongs",
                newName: "IX_PlaylistSongs_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistSongs_AspNetUsers_UserId",
                table: "PlaylistSongs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistSongs_AspNetUsers_UserId",
                table: "PlaylistSongs");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "PlaylistSongs",
                newName: "AddedBy");

            migrationBuilder.RenameIndex(
                name: "IX_PlaylistSongs_UserId",
                table: "PlaylistSongs",
                newName: "IX_PlaylistSongs_AddedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistSongs_AspNetUsers_AddedBy",
                table: "PlaylistSongs",
                column: "AddedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
