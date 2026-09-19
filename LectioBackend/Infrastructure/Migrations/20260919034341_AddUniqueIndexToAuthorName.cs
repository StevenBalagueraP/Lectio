using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LectioBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToAuthorName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Author_AuthorName",
                table: "Authors",
                column: "AuthorName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Author_AuthorName",
                table: "Authors");
        }
    }
}
