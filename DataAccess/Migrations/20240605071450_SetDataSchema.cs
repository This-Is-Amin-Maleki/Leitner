using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLeit.Migrations
{
    /// <inheritdoc />
    public partial class SetDataSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.RenameTable(
                name: "Slots",
                newName: "Slots",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Containers",
                newName: "Containers",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ContainerCards",
                newName: "ContainerCards",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Collections",
                newName: "Collections",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Cards",
                newName: "Cards",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Boxes",
                newName: "Boxes",
                newSchema: "dbo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Slots",
                schema: "dbo",
                newName: "Slots");

            migrationBuilder.RenameTable(
                name: "Containers",
                schema: "dbo",
                newName: "Containers");

            migrationBuilder.RenameTable(
                name: "ContainerCards",
                schema: "dbo",
                newName: "ContainerCards");

            migrationBuilder.RenameTable(
                name: "Collections",
                schema: "dbo",
                newName: "Collections");

            migrationBuilder.RenameTable(
                name: "Cards",
                schema: "dbo",
                newName: "Cards");

            migrationBuilder.RenameTable(
                name: "Boxes",
                schema: "dbo",
                newName: "Boxes");
        }
    }
}
