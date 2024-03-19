using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sarvicny.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addingOrdersColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomerID",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_Carts_CartID",
                table: "ServiceRequests");

            migrationBuilder.RenameColumn(
                name: "isVerified",
                table: "ServiceProviders",
                newName: "IsVerified");

            migrationBuilder.AlterColumn<string>(
                name: "CartID",
                table: "ServiceRequests",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ProblemDescription",
                table: "ServiceRequests",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ProviderDistrictID",
                table: "ServiceRequests",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "ServiceProviders",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "Orders",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderDate",
                table: "Orders",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TransactionID",
                table: "Orders",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "salary",
                table: "Consultants",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Districts",
                columns: table => new
                {
                    DistrictID = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DistrictName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Availability = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.DistrictID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OrderRatings",
                columns: table => new
                {
                    RatingId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OrderId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CustomerId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CustomerRating = table.Column<int>(type: "int", nullable: true),
                    ServiceProviderRating = table.Column<int>(type: "int", nullable: true),
                    Comment = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderRatings", x => x.RatingId);
                    table.ForeignKey(
                        name: "FK_OrderRatings_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderRatings_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderRatings_ServiceProviders_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "ServiceProviders",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProviderDistricts",
                columns: table => new
                {
                    ProviderDistrictID = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderID = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DistrictID = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    enable = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderDistricts", x => x.ProviderDistrictID);
                    table.ForeignKey(
                        name: "FK_ProviderDistricts_Districts_DistrictID",
                        column: x => x.DistrictID,
                        principalTable: "Districts",
                        principalColumn: "DistrictID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProviderDistricts_ServiceProviders_ProviderID",
                        column: x => x.ProviderID,
                        principalTable: "ServiceProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_ProviderDistrictID",
                table: "ServiceRequests",
                column: "ProviderDistrictID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderRatings_CustomerId",
                table: "OrderRatings",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderRatings_OrderId",
                table: "OrderRatings",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderRatings_ProviderId",
                table: "OrderRatings",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderDistricts_DistrictID",
                table: "ProviderDistricts",
                column: "DistrictID");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderDistricts_ProviderID",
                table: "ProviderDistricts",
                column: "ProviderID");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customers_CustomerID",
                table: "Orders",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_Carts_CartID",
                table: "ServiceRequests",
                column: "CartID",
                principalTable: "Carts",
                principalColumn: "CartID");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_ProviderDistricts_ProviderDistrictID",
                table: "ServiceRequests",
                column: "ProviderDistrictID",
                principalTable: "ProviderDistricts",
                principalColumn: "ProviderDistrictID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomerID",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_Carts_CartID",
                table: "ServiceRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_ProviderDistricts_ProviderDistrictID",
                table: "ServiceRequests");

            migrationBuilder.DropTable(
                name: "OrderRatings");

            migrationBuilder.DropTable(
                name: "ProviderDistricts");

            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRequests_ProviderDistrictID",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "ProblemDescription",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "ProviderDistrictID",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "ServiceProviders");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TransactionID",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "salary",
                table: "Consultants");

            migrationBuilder.RenameColumn(
                name: "IsVerified",
                table: "ServiceProviders",
                newName: "isVerified");

            migrationBuilder.UpdateData(
                table: "ServiceRequests",
                keyColumn: "CartID",
                keyValue: null,
                column: "CartID",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "CartID",
                table: "ServiceRequests",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customers_CustomerID",
                table: "Orders",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_Carts_CartID",
                table: "ServiceRequests",
                column: "CartID",
                principalTable: "Carts",
                principalColumn: "CartID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
