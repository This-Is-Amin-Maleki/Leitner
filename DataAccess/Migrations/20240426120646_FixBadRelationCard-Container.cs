using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixBadRelationCardContainer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_Containers_ContainerId",
                table: "Cards");

            migrationBuilder.DropIndex(
                name: "IX_Cards_ContainerId",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "ContainerId",
                table: "Cards");

            migrationBuilder.CreateTable(
                name: "CardContainer",
                columns: table => new
                {
                    CardsId = table.Column<long>(type: "bigint", nullable: false),
                    ContainersId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardContainer", x => new { x.CardsId, x.ContainersId });
                    table.ForeignKey(
                        name: "FK_CardContainer_Cards_CardsId",
                        column: x => x.CardsId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardContainer_Containers_ContainersId",
                        column: x => x.ContainersId,
                        principalTable: "Containers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardContainer_ContainersId",
                table: "CardContainer",
                column: "ContainersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardContainer");

            migrationBuilder.AddColumn<long>(
                name: "ContainerId",
                table: "Cards",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Cards_ContainerId",
                table: "Cards",
                column: "ContainerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_Containers_ContainerId",
                table: "Cards",
                column: "ContainerId",
                principalTable: "Containers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
