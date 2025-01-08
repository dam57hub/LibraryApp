using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DamianMatysik_BDwAI_Project.Migrations
{
    /// <inheritdoc />
    public partial class addcustomuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Borrowings_Books_BookId",
                table: "Borrowings");

            migrationBuilder.DropForeignKey(
                name: "FK_Borrowings_Borrowers_BorrowerId",
                table: "Borrowings");

            migrationBuilder.DropTable(
                name: "Borrowers");

            migrationBuilder.DropIndex(
                name: "IX_Borrowings_BorrowerId",
                table: "Borrowings");

            migrationBuilder.DropColumn(
                name: "BorrowerId",
                table: "Borrowings");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Borrowings",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Borrowings_UserId",
                table: "Borrowings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Borrowings_AspNetUsers_UserId",
                table: "Borrowings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Borrowings_Books_BookId",
                table: "Borrowings",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Borrowings_AspNetUsers_UserId",
                table: "Borrowings");

            migrationBuilder.DropForeignKey(
                name: "FK_Borrowings_Books_BookId",
                table: "Borrowings");

            migrationBuilder.DropIndex(
                name: "IX_Borrowings_UserId",
                table: "Borrowings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Borrowings");

            migrationBuilder.AddColumn<int>(
                name: "BorrowerId",
                table: "Borrowings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Borrowers",
                columns: table => new
                {
                    BorrowerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FullName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Borrowers", x => x.BorrowerId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Borrowings_BorrowerId",
                table: "Borrowings",
                column: "BorrowerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Borrowings_Books_BookId",
                table: "Borrowings",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Borrowings_Borrowers_BorrowerId",
                table: "Borrowings",
                column: "BorrowerId",
                principalTable: "Borrowers",
                principalColumn: "BorrowerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
