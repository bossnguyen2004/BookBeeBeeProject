using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookBee.Migrations
{
    public partial class updateImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Books_BookId",
                table: "Images");

            migrationBuilder.AlterColumn<int>(
                name: "BookId",
                table: "Images",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TagId",
                table: "Images",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Images_TagId",
                table: "Images",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Books_BookId",
                table: "Images",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Tags_TagId",
                table: "Images",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Books_BookId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Tags_TagId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_TagId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "Images");

            migrationBuilder.AlterColumn<int>(
                name: "BookId",
                table: "Images",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Books_BookId",
                table: "Images",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id");
        }
    }
}
