using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Assignment01.Migrations
{
    /// <inheritdoc />
    public partial class AddDummyData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "Id", "AvailableTickets", "Category", "EventDate", "PricePerTicket", "Title" },
                values: new object[,]
                {
                    { 1, 100, "Workshop", new DateTime(2025, 10, 1, 0, 0, 0, 0, DateTimeKind.Utc), 50.0, "Event 1" },
                    { 2, 100, "Concert", new DateTime(2025, 11, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0.0, "Event 2" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
