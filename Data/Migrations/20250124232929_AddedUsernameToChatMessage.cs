using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicMatch.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedUsernameToChatMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "ChatMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "ChatMessages");
        }
    }
}
