using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StyleHubApi.Migrations
{
    /// <inheritdoc />
    public partial class vc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Orders",
                newName: "UsertId");

            migrationBuilder.AddColumn<string>(
                name: "AddressId",
                table: "Orders",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "Addresses",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_OrderId",
                table: "Addresses",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Orders_OrderId",
                table: "Addresses",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                table: "OrderItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Orders_OrderId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_OrderId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "UsertId",
                table: "Orders",
                newName: "Address");
        }
    }
}
