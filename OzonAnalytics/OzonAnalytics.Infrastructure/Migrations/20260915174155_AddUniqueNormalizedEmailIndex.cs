using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OzonAnalytics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueNormalizedEmailIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUsers_NormalizedEmail",
                table: "ApplicationUsers",
                column: "NormalizedEmail",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApplicationUsers_NormalizedEmail",
                table: "ApplicationUsers");
        }
    }
}
