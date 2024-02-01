using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sarvicny.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class editingOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_ProviderServices_providerServiceProviderID_p~",
                table: "ServiceRequests");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRequests_providerServiceProviderID_providerServiceSer~",
                table: "ServiceRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProviderServices",
                table: "ProviderServices");

            migrationBuilder.DropColumn(
                name: "providerServiceProviderID",
                table: "ServiceRequests");

            //migrationBuilder.RenameColumn(
            //    name: "providerServiceServiceID",
            //    table: "ServiceRequests",
            //    newName: "OrderId");

            migrationBuilder.AlterColumn<string>(
                name: "SlotID",
                table: "ServiceRequests",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderServiceID",
                table: "ServiceRequests",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "ProviderServices",
                keyColumn: "ProviderServiceID",
                keyValue: null,
                column: "ProviderServiceID",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderServiceID",
                table: "ProviderServices",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProviderServices",
                table: "ProviderServices",
                column: "ProviderServiceID");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_OrderId",
                table: "ServiceRequests",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_ProviderServiceID",
                table: "ServiceRequests",
                column: "ProviderServiceID");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_SlotID",
                table: "ServiceRequests",
                column: "SlotID");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderServices_ProviderID",
                table: "ProviderServices",
                column: "ProviderID");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_Orders_OrderId",
                table: "ServiceRequests",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_ProviderServices_ProviderServiceID",
                table: "ServiceRequests",
                column: "ProviderServiceID",
                principalTable: "ProviderServices",
                principalColumn: "ProviderServiceID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_Slots_SlotID",
                table: "ServiceRequests",
                column: "SlotID",
                principalTable: "Slots",
                principalColumn: "TimeSlotID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_Orders_OrderId",
                table: "ServiceRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_ProviderServices_ProviderServiceID",
                table: "ServiceRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_Slots_SlotID",
                table: "ServiceRequests");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRequests_OrderId",
                table: "ServiceRequests");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRequests_ProviderServiceID",
                table: "ServiceRequests");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRequests_SlotID",
                table: "ServiceRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProviderServices",
                table: "ProviderServices");

            migrationBuilder.DropIndex(
                name: "IX_ProviderServices_ProviderID",
                table: "ProviderServices");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "ServiceRequests",
                newName: "providerServiceServiceID");

            migrationBuilder.AlterColumn<string>(
                name: "SlotID",
                table: "ServiceRequests",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderServiceID",
                table: "ServiceRequests",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "providerServiceProviderID",
                table: "ServiceRequests",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderServiceID",
                table: "ProviderServices",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProviderServices",
                table: "ProviderServices",
                columns: new[] { "ProviderID", "ServiceID" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_providerServiceProviderID_providerServiceSer~",
                table: "ServiceRequests",
                columns: new[] { "providerServiceProviderID", "providerServiceServiceID" });

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_ProviderServices_providerServiceProviderID_p~",
                table: "ServiceRequests",
                columns: new[] { "providerServiceProviderID", "providerServiceServiceID" },
                principalTable: "ProviderServices",
                principalColumns: new[] { "ProviderID", "ServiceID" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
