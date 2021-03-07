using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseWork.Data.Migrations
{
    public partial class Add_Chapters_set : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chapter_Books_BookId",
                table: "Chapter");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Chapter",
                table: "Chapter");

            migrationBuilder.RenameTable(
                name: "Chapter",
                newName: "Chapters");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Chapters",
                table: "Chapters",
                columns: new[] { "BookId", "ChapterNum" });

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_Books_BookId",
                table: "Chapters",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_Books_BookId",
                table: "Chapters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Chapters",
                table: "Chapters");

            migrationBuilder.RenameTable(
                name: "Chapters",
                newName: "Chapter");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Chapter",
                table: "Chapter",
                columns: new[] { "BookId", "ChapterNum" });

            migrationBuilder.AddForeignKey(
                name: "FK_Chapter_Books_BookId",
                table: "Chapter",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
