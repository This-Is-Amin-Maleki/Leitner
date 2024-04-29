using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLeit.Migrations
{
    /// <inheritdoc />
    public partial class AddCompletedPropertyToBox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "Collections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                table: "Boxes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "Completed",
                table: "Boxes");
        }
    }
}
