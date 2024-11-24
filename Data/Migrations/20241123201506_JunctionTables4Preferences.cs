using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicMatch.Data.Migrations
{
    /// <inheritdoc />
    public partial class JunctionTables4Preferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPreferencesForms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferencesForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPreferencesForms_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPreferencesArtists",
                columns: table => new
                {
                    UserPreferencesFormId = table.Column<int>(type: "int", nullable: false),
                    ArtistId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferencesArtists", x => new { x.UserPreferencesFormId, x.ArtistId });
                    table.ForeignKey(
                        name: "FK_UserPreferencesArtists_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPreferencesArtists_UserPreferencesForms_UserPreferencesFormId",
                        column: x => x.UserPreferencesFormId,
                        principalTable: "UserPreferencesForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPreferencesSongs",
                columns: table => new
                {
                    UserPreferencesFormId = table.Column<int>(type: "int", nullable: false),
                    SongId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferencesSongs", x => new { x.UserPreferencesFormId, x.SongId });
                    table.ForeignKey(
                        name: "FK_UserPreferencesSongs_Songs_SongId",
                        column: x => x.SongId,
                        principalTable: "Songs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPreferencesSongs_UserPreferencesForms_UserPreferencesFormId",
                        column: x => x.UserPreferencesFormId,
                        principalTable: "UserPreferencesForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferencesArtists_ArtistId",
                table: "UserPreferencesArtists",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferencesForms_UserId",
                table: "UserPreferencesForms",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferencesSongs_SongId",
                table: "UserPreferencesSongs",
                column: "SongId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPreferencesArtists");

            migrationBuilder.DropTable(
                name: "UserPreferencesSongs");

            migrationBuilder.DropTable(
                name: "UserPreferencesForms");
        }
    }
}
