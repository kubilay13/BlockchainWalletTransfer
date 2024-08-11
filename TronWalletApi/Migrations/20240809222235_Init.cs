using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TronWalletApi.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Networks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    NetworkId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Networks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Contract = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Decimal = table.Column<int>(type: "int", nullable: false),
                    Commission = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    AdminWallet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdminWalletPrivateKey = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Networks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionSuccesHistoryModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SendingAddress = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false),
                    ReceivedAddress = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false),
                    TransactionHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CoinType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TransactionNetwork = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TransactionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionDateTime = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    Commission = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    NetworkFee = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    SenderTransactionUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverTransactionUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionStatus = table.Column<bool>(type: "bit", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    Network = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionSuccesHistoryModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransferHistoryModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SendingAddress = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false),
                    ReceivedAddress = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false),
                    TransactionHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CoinType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TransactionNetwork = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TransactionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionDateTime = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    Commission = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    NetworkFee = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    SenderTransactionUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverTransactionUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionStatus = table.Column<bool>(type: "bit", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    Network = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferHistoryModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TronWalletModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WalletName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PrivateKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    WalletAddress = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAtTime = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    LastTransactionAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastTransactionTime = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    TrxAmount = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    UsdtAmount = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    UsdcAmount = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    ETHAmount = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    Network = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WalletTronScanURL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TransactionLimit = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TronWalletModels", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Networks",
                columns: new[] { "Id", "AdminWallet", "AdminWalletPrivateKey", "Commission", "Contract", "Decimal", "Name", "NetworkId", "Networks", "Type" },
                values: new object[,]
                {
                    { 1, "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N", "0107932b30922231adff71b4b7c0b05bc948632f56c2b62f98bd18fefeae8a9e", 10m, null, 6, "TRX", null, "TRON", 0 },
                    { 2, "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N", "0107932b30922231adff71b4b7c0b05bc948632f56c2b62f98bd18fefeae8a9e", 10m, "TXYZopYRdj2D9XRtbG411XZZ3kM5VkAeBf", 6, "USDT", null, "TRON", 0 },
                    { 3, "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N", "0107932b30922231adff71b4b7c0b05bc948632f56c2b62f98bd18fefeae8a9e", 10m, "TEMVynQpntMqkPxP6wXTW2K7e4sM3cRmWz", 6, "USDC", null, "TRON", 0 }
                });

            migrationBuilder.InsertData(
                table: "TronWalletModels",
                columns: new[] { "Id", "CreatedAt", "CreatedAtTime", "ETHAmount", "LastTransactionAt", "LastTransactionTime", "Network", "PrivateKey", "TransactionLimit", "TrxAmount", "UsdcAmount", "UsdtAmount", "WalletAddress", "WalletName", "WalletTronScanURL" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 8, 9, 22, 22, 34, 443, DateTimeKind.Utc).AddTicks(7503), "01:22:34", 0m, new DateTime(2024, 8, 9, 22, 22, 34, 443, DateTimeKind.Utc).AddTicks(7504), "10:49:03", "Testnet(Nile)", "5a87ccab1b8b8f2d86c24ad6f278d8030be5a17d056588242ef377d9c3ddeb8e", false, 0m, 0m, 0m, "TXTVwsUMsWrWsvd61VRcE9Bsk4WbEY9DGv", "TestAdress", "https://nile.tronscan.org/#/address/TXTVwsUMsWrWsvd61VRcE9Bsk4WbEY9DGv" },
                    { 2, new DateTime(2024, 8, 9, 22, 22, 34, 443, DateTimeKind.Utc).AddTicks(7513), "01:22:34", 0m, new DateTime(2024, 8, 9, 22, 22, 34, 443, DateTimeKind.Utc).AddTicks(7514), "10:49:03", "Testnet(Nile)", "0107932b30922231adff71b4b7c0b05bc948632f56c2b62f98bd18fefeae8a9e", true, 0m, 0m, 0m, "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N", "AdminAdress", "https://nile.tronscan.org/#/address/TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Networks");

            migrationBuilder.DropTable(
                name: "TransactionSuccesHistoryModels");

            migrationBuilder.DropTable(
                name: "TransferHistoryModels");

            migrationBuilder.DropTable(
                name: "TronWalletModels");
        }
    }
}
