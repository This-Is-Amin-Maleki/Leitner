using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLeit.Migrations
{
    /// <inheritdoc />
    public partial class CascadeContainerConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContainerCards_Containers_ContainerId",
                table: "ContainerCards");

            migrationBuilder.AddForeignKey(
                name: "FK_ContainerCards_Containers_ContainerId",
                table: "ContainerCards",
                column: "ContainerId",
                principalTable: "Containers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContainerCards_Containers_ContainerId",
                table: "ContainerCards");

            migrationBuilder.AddForeignKey(
                name: "FK_ContainerCards_Containers_ContainerId",
                table: "ContainerCards",
                column: "ContainerId",
                principalTable: "Containers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
