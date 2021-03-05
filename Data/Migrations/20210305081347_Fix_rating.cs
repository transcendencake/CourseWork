using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseWork.Data.Migrations
{
    public partial class Fix_rating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rating_Book_BookId1",
                table: "Rating");

            migrationBuilder.DropIndex(
                name: "IX_Rating_BookId1",
                table: "Rating");

            migrationBuilder.DropColumn(
                name: "BookId1",
                table: "Rating");

            migrationBuilder.AlterColumn<int>(
                name: "BookId",
                table: "Rating",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_BookId",
                table: "Rating",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_Book_BookId",
                table: "Rating",
                column: "BookId",
                principalTable: "Book",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rating_Book_BookId",
                table: "Rating");

            migrationBuilder.DropIndex(
                name: "IX_Rating_BookId",
                table: "Rating");

            migrationBuilder.AlterColumn<string>(
                name: "BookId",
                table: "Rating",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "BookId1",
                table: "Rating",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rating_BookId1",
                table: "Rating",
                column: "BookId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_Book_BookId1",
                table: "Rating",
                column: "BookId1",
                principalTable: "Book",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
