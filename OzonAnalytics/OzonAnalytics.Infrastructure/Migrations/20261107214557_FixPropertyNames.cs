using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OzonAnalytics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPropertyNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InProccesAt",
                table: "Orders",
                newName: "InProcessAt");

            migrationBuilder.RenameColumn(
                name: "CommisionAmount",
                table: "OrderItems",
                newName: "CommissionAmount");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShopId_InProcessAt",
                table: "Orders",
                columns: new[] { "ShopId", "InProcessAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_ShopId_InProcessAt",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "InProcessAt",
                table: "Orders",
                newName: "InProccesAt");

            migrationBuilder.RenameColumn(
                name: "CommissionAmount",
                table: "OrderItems",
                newName: "CommisionAmount");
        }
    }
}
