using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HabbitTrackerRazor.Migrations
{
    /// <inheritdoc />
    public partial class AddMonthYearToEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "MemorableMoments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "MemorableMoments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "HabitEntries",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "HabitEntries",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Month",
                table: "MemorableMoments");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "MemorableMoments");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "HabitEntries");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "HabitEntries");
        }
    }
}
