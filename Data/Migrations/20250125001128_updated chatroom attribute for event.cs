using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicMatch.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatedchatroomattributeforevent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatRooms_Events_EventId",
                table: "ChatRooms");

            migrationBuilder.DropIndex(
                name: "IX_ChatRooms_EventId",
                table: "ChatRooms");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "ChatRooms");

            migrationBuilder.AddColumn<int>(
                name: "ChatRoomId",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_ChatRoomId",
                table: "Events",
                column: "ChatRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_ChatRooms_ChatRoomId",
                table: "Events",
                column: "ChatRoomId",
                principalTable: "ChatRooms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_ChatRooms_ChatRoomId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_ChatRoomId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ChatRoomId",
                table: "Events");

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "ChatRooms",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_EventId",
                table: "ChatRooms",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRooms_Events_EventId",
                table: "ChatRooms",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");
        }
    }
}
