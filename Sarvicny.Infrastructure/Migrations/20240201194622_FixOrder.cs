using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sarvicny.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_Carts_CartID",
                table: "ServiceRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_Carts_CartID",
                table: "ServiceRequests",
                column: "CartID",
                principalTable: "Carts",
                principalColumn: "CartID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_Carts_CartID",
                table: "ServiceRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_Carts_CartID",
                table: "ServiceRequests",
                column: "CartID",
                principalTable: "Carts",
                principalColumn: "CartID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
