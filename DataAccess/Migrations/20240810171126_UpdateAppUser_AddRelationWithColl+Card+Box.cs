using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLeit.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAppUser_AddRelationWithCollCardBox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserId",
                schema: "dbo",
                table: "Collections",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                schema: "dbo",
                table: "Cards",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                schema: "dbo",
                table: "Boxes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Bio",
                schema: "dbo",
                table: "AspNetUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "dbo",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_UserId",
                schema: "dbo",
                table: "Collections",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_UserId",
                schema: "dbo",
                table: "Cards",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Boxes_UserId",
                schema: "dbo",
                table: "Boxes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Boxes_AspNetUsers_UserId",
                schema: "dbo",
                table: "Boxes",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_AspNetUsers_UserId",
                schema: "dbo",
                table: "Cards",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Collections_AspNetUsers_UserId",
                schema: "dbo",
                table: "Collections",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boxes_AspNetUsers_UserId",
                schema: "dbo",
                table: "Boxes");

            migrationBuilder.DropForeignKey(
                name: "FK_Cards_AspNetUsers_UserId",
                schema: "dbo",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_Collections_AspNetUsers_UserId",
                schema: "dbo",
                table: "Collections");

            migrationBuilder.DropIndex(
                name: "IX_Collections_UserId",
                schema: "dbo",
                table: "Collections");

            migrationBuilder.DropIndex(
                name: "IX_Cards_UserId",
                schema: "dbo",
                table: "Cards");

            migrationBuilder.DropIndex(
                name: "IX_Boxes_UserId",
                schema: "dbo",
                table: "Boxes");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "dbo",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "dbo",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "dbo",
                table: "Boxes");

            migrationBuilder.DropColumn(
                name: "Bio",
                schema: "dbo",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "dbo",
                table: "AspNetUsers");
        }
    }
}
