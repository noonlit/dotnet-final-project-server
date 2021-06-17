using Microsoft.EntityFrameworkCore.Migrations;

namespace FinalProject.Migrations
{
    public partial class ConvertStoryAuthorToOwner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserStory");

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Stories",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stories_OwnerId",
                table: "Stories",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stories_AspNetUsers_OwnerId",
                table: "Stories",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stories_AspNetUsers_OwnerId",
                table: "Stories");

            migrationBuilder.DropIndex(
                name: "IX_Stories_OwnerId",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Stories");

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
    }
}
