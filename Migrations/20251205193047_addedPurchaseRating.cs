using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assignment01.Migrations
{
    /// <inheritdoc />
    public partial class addedPurchaseRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PurchaseRating",
                table: "Purchases",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchaseRating",
                table: "Purchases");
        }
    }
}
