using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicMatch.Data.Migrations
{
    /// <inheritdoc />
    public partial class CorrectedAppDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserChatRoom_AspNetUsers_UserId",
                table: "UserChatRoom");

            migrationBuilder.DropForeignKey(
                name: "FK_UserChatRoom_ChatRooms_ChatRoomId",
                table: "UserChatRoom");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserChatRoom",
                table: "UserChatRoom");

            migrationBuilder.RenameTable(
                name: "UserChatRoom",
                newName: "UserChatRooms");

            migrationBuilder.RenameIndex(
                name: "IX_UserChatRoom_UserId",
                table: "UserChatRooms",
                newName: "IX_UserChatRooms_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserChatRoom_ChatRoomId",
                table: "UserChatRooms",
                newName: "IX_UserChatRooms_ChatRoomId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserChatRooms",
                table: "UserChatRooms",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserChatRooms_AspNetUsers_UserId",
                table: "UserChatRooms",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserChatRooms_ChatRooms_ChatRoomId",
                table: "UserChatRooms",
                column: "ChatRoomId",
                principalTable: "ChatRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserChatRooms_AspNetUsers_UserId",
                table: "UserChatRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_UserChatRooms_ChatRooms_ChatRoomId",
                table: "UserChatRooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserChatRooms",
                table: "UserChatRooms");

            migrationBuilder.RenameTable(
                name: "UserChatRooms",
                newName: "UserChatRoom");

            migrationBuilder.RenameIndex(
                name: "IX_UserChatRooms_UserId",
                table: "UserChatRoom",
                newName: "IX_UserChatRoom_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserChatRooms_ChatRoomId",
                table: "UserChatRoom",
                newName: "IX_UserChatRoom_ChatRoomId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserChatRoom",
                table: "UserChatRoom",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserChatRoom_AspNetUsers_UserId",
                table: "UserChatRoom",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserChatRoom_ChatRooms_ChatRoomId",
                table: "UserChatRoom",
                column: "ChatRoomId",
                principalTable: "ChatRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
