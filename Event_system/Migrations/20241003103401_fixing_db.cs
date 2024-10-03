using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Event_system.Migrations
{
    /// <inheritdoc />
    public partial class fixing_db : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "stars",
                table: "Ratings",
                newName: "Stars");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Stars",
                table: "Ratings",
                newName: "stars");
        }
    }
}
