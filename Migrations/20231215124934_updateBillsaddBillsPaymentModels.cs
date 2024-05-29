using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LodgeLink.Migrations
{
    /// <inheritdoc />
    public partial class updateBillsaddBillsPaymentModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bill_residents_ResidentId",
                table: "Bill");

            migrationBuilder.DropIndex(
                name: "IX_Bill_ResidentId",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Bill");

            migrationBuilder.RenameColumn(
                name: "ResidentId",
                table: "Bill",
                newName: "Unpaid");

            migrationBuilder.RenameColumn(
                name: "PaymentStatus",
                table: "Bill",
                newName: "DateRange");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "buildings",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "buildings",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "Paid",
                table: "Bill",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Bill",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "bills_payment",
                columns: table => new
                {
                    BillPaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillId = table.Column<int>(type: "int", nullable: false),
                    ResidentId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    PaymentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaidAmount = table.Column<double>(type: "float", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    PaidOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bills_payment", x => x.BillPaymentId);
                    table.ForeignKey(
                        name: "FK_bills_payment_Bill_BillId",
                        column: x => x.BillId,
                        principalTable: "Bill",
                        principalColumn: "BillId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_bills_payment_residents_ResidentId",
                        column: x => x.ResidentId,
                        principalTable: "residents",
                        principalColumn: "ResidentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_bills_payment_BillId",
                table: "bills_payment",
                column: "BillId");

            migrationBuilder.CreateIndex(
                name: "IX_bills_payment_ResidentId",
                table: "bills_payment",
                column: "ResidentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bills_payment");

            migrationBuilder.DropColumn(
                name: "Paid",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Bill");

            migrationBuilder.RenameColumn(
                name: "Unpaid",
                table: "Bill",
                newName: "ResidentId");

            migrationBuilder.RenameColumn(
                name: "DateRange",
                table: "Bill",
                newName: "PaymentStatus");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "buildings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "buildings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Bill",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Bill_ResidentId",
                table: "Bill",
                column: "ResidentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bill_residents_ResidentId",
                table: "Bill",
                column: "ResidentId",
                principalTable: "residents",
                principalColumn: "ResidentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
