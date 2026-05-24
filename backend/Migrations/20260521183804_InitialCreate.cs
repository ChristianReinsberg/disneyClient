using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DisneyApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "characters",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    image_url = table.Column<string>(type: "text", nullable: false),
                    films = table.Column<string>(type: "text", nullable: false),
                    short_films = table.Column<string>(type: "text", nullable: false),
                    tv_shows = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_characters", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "medias",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    media_type = table.Column<string>(type: "text", nullable: false),
                    overview = table.Column<string>(type: "text", nullable: false),
                    poster_path = table.Column<string>(type: "text", nullable: false),
                    release_date = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    vote_avg = table.Column<float>(type: "real", nullable: false),
                    vote_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_medias", x => new { x.id, x.media_type });
                });

            migrationBuilder.CreateTable(
                name: "character_media",
                columns: table => new
                {
                    character_id = table.Column<int>(type: "integer", nullable: false),
                    medi_id = table.Column<int>(type: "integer", nullable: false),
                    media_type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_character_media", x => new { x.character_id, x.medi_id, x.media_type });
                    table.ForeignKey(
                        name: "fk_character_media_characters_character_id",
                        column: x => x.character_id,
                        principalTable: "characters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_character_media_medias_medi_id_media_type",
                        columns: x => new { x.medi_id, x.media_type },
                        principalTable: "medias",
                        principalColumns: new[] { "id", "media_type" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_character_media_medi_id_media_type",
                table: "character_media",
                columns: new[] { "medi_id", "media_type" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_media");

            migrationBuilder.DropTable(
                name: "characters");

            migrationBuilder.DropTable(
                name: "medias");
        }
    }
}
