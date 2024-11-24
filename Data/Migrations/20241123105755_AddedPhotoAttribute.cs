using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicMatch.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedPhotoAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserChatRooms_AspNetUsers_ParticipantsId",
                table: "UserChatRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_UserChatRooms_ChatRooms_ChatRoomsId",
                table: "UserChatRooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserChatRooms",
                table: "UserChatRooms");

            migrationBuilder.RenameTable(
                name: "UserChatRooms",
                newName: "ApplicationUserChatRoom");

            migrationBuilder.RenameIndex(
                name: "IX_UserChatRooms_ParticipantsId",
                table: "ApplicationUserChatRoom",
                newName: "IX_ApplicationUserChatRoom_ParticipantsId");

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "Artists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUserChatRoom",
                table: "ApplicationUserChatRoom",
                columns: new[] { "ChatRoomsId", "ParticipantsId" });

            migrationBuilder.CreateTable(
                name: "UserChatRoom",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChatRoomId = table.Column<int>(type: "int", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserChatRoom", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserChatRoom_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserChatRoom_ChatRooms_ChatRoomId",
                        column: x => x.ChatRoomId,
                        principalTable: "ChatRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSongs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SongId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSongs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSongs_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserSongs_Songs_SongId",
                        column: x => x.SongId,
                        principalTable: "Songs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserChatRoom_ChatRoomId",
                table: "UserChatRoom",
                column: "ChatRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChatRoom_UserId",
                table: "UserChatRoom",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSongs_ApplicationUserId",
                table: "UserSongs",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSongs_SongId",
                table: "UserSongs",
                column: "SongId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserChatRoom_AspNetUsers_ParticipantsId",
                table: "ApplicationUserChatRoom",
                column: "ParticipantsId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserChatRoom_ChatRooms_ChatRoomsId",
                table: "ApplicationUserChatRoom",
                column: "ChatRoomsId",
                principalTable: "ChatRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserChatRoom_AspNetUsers_ParticipantsId",
                table: "ApplicationUserChatRoom");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserChatRoom_ChatRooms_ChatRoomsId",
                table: "ApplicationUserChatRoom");

            migrationBuilder.DropTable(
                name: "UserChatRoom");

            migrationBuilder.DropTable(
                name: "UserSongs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUserChatRoom",
                table: "ApplicationUserChatRoom");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "Artists");

            migrationBuilder.RenameTable(
                name: "ApplicationUserChatRoom",
                newName: "UserChatRooms");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserChatRoom_ParticipantsId",
                table: "UserChatRooms",
                newName: "IX_UserChatRooms_ParticipantsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserChatRooms",
                table: "UserChatRooms",
                columns: new[] { "ChatRoomsId", "ParticipantsId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserChatRooms_AspNetUsers_ParticipantsId",
                table: "UserChatRooms",
                column: "ParticipantsId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserChatRooms_ChatRooms_ChatRoomsId",
                table: "UserChatRooms",
                column: "ChatRoomsId",
                principalTable: "ChatRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
