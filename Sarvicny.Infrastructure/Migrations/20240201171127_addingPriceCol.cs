using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sarvicny.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addingPriceCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_Orders_OrderId",
                table: "ServiceRequests");

            migrationBuilder.AlterColumn<string>(
                name: "OrderId",
                table: "ServiceRequests",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "ServiceRequests",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_Orders_OrderId",
                table: "ServiceRequests",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_Orders_OrderId",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "ServiceRequests");

            migrationBuilder.UpdateData(
                table: "ServiceRequests",
                keyColumn: "OrderId",
                keyValue: null,
                column: "OrderId",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "OrderId",
                table: "ServiceRequests",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_Orders_OrderId",
                table: "ServiceRequests",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
