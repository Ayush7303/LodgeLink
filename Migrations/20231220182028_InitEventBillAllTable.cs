using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LodgeLink.Migrations
{
    /// <inheritdoc />
    public partial class InitEventBillAllTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Feedback",
                table: "eventParticipants",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<double>(
                name: "PaidAmount",
                table: "bookingRequests",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentId",
                table: "bookingRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LateCharge",
                table: "bills_payment",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "bookingRequests");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "bookingRequests");

            migrationBuilder.DropColumn(
                name: "LateCharge",
                table: "bills_payment");

            migrationBuilder.AlterColumn<string>(
                name: "Feedback",
                table: "eventParticipants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
