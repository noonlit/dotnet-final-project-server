using Microsoft.EntityFrameworkCore.Migrations;

namespace FinalProject.Migrations
{
    public partial class FixStoryUserRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Stories_StoryId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_StoryId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StoryId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "ApplicationUserStory",
                columns: table => new
                {
                    AuthorsId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StoriesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserStory", x => new { x.AuthorsId, x.StoriesId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserStory_AspNetUsers_AuthorsId",
                        column: x => x.AuthorsId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserStory_Stories_StoriesId",
                        column: x => x.StoriesId,
                        principalTable: "Stories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserStory_StoriesId",
                table: "ApplicationUserStory",
                column: "StoriesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserStory");

            migrationBuilder.AddColumn<int>(
                name: "StoryId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_StoryId",
                table: "AspNetUsers",
                column: "StoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Stories_StoryId",
                table: "AspNetUsers",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
