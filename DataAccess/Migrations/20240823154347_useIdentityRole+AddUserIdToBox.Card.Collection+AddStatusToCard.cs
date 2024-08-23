using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLeit.Migrations
{
    /// <inheritdoc />
    public partial class useIdentityRoleAddUserIdToBoxCardCollectionAddStatusToCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserType",
                schema: "dbo",
                table: "AspNetRoles");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "dbo",
                table: "Cards",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                schema: "dbo",
                table: "Cards");

            migrationBuilder.AddColumn<int>(
                name: "UserType",
                schema: "dbo",
                table: "AspNetRoles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
