using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LodgeLink.Migrations
{
    /// <inheritdoc />
    public partial class UpdateArchitecturalTB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PropertyId",
                table: "architecturalRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_architecturalRequests_PropertyId",
                table: "architecturalRequests",
                column: "PropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_architecturalRequests_properties_PropertyId",
                table: "architecturalRequests",
                column: "PropertyId",
                principalTable: "properties",
                principalColumn: "PropertyId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_architecturalRequests_properties_PropertyId",
                table: "architecturalRequests");

            migrationBuilder.DropIndex(
                name: "IX_architecturalRequests_PropertyId",
                table: "architecturalRequests");

            migrationBuilder.DropColumn(
                name: "PropertyId",
                table: "architecturalRequests");
        }
    }
}
