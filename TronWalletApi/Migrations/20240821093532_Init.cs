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
                name: "AdminLoginModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminLoginModels", x => x.Id);
                });

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
                name: "TransferHistoryModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SendingAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ReceivedAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    TransactionHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CoinType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TransactionNetwork = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TransactionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Commission = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    NetworkFee = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
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
                name: "userLoginModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserMailAdress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userLoginModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSignUpModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TelNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WalletName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSignUpModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WalletModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TelNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WalletName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastTransactionAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionLimit = table.Column<bool>(type: "bit", nullable: false),
                    Network = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WalletDetailModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PrivateKeyTron = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    WalletAddressTron = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false),
                    TrxAmount = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    UsdtAmount = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    UsdcAmount = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    UsddAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrivateKeyEth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WalletAddressETH = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ETHAmount = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    WalletScanURL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    WalletId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletDetailModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletDetailModels_WalletModels_WalletId",
                        column: x => x.WalletId,
                        principalTable: "WalletModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                table: "WalletModels",
                columns: new[] { "Id", "AccountName", "CreatedAt", "Email", "LastTransactionAt", "Name", "Network", "Password", "Surname", "TelNo", "TransactionLimit", "UserId", "WalletName" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2024, 8, 21, 9, 35, 31, 529, DateTimeKind.Utc).AddTicks(5706), "user@example.com", new DateTime(2024, 8, 21, 9, 35, 31, 529, DateTimeKind.Utc).AddTicks(5717), "TRXAdminAdress", "Testnet(Nile)", "Password", "SurnameAdminTRX", "stringstri", true, 0, "TRXAdminAdress" },
                    { 2, null, new DateTime(2024, 8, 21, 9, 35, 31, 529, DateTimeKind.Utc).AddTicks(5787), "user@example.com", new DateTime(2024, 8, 21, 9, 35, 31, 529, DateTimeKind.Utc).AddTicks(5787), "ETHAdminAdress", "TestNet(Sepolia)", "Password", "SurnameAdminTETH", "stringstri", true, 0, "ETHAdminAdress" }
                });

            migrationBuilder.InsertData(
                table: "WalletDetailModels",
                columns: new[] { "Id", "ETHAmount", "PrivateKeyEth", "PrivateKeyTron", "TrxAmount", "UsdcAmount", "UsddAmount", "UsdtAmount", "UserId", "WalletAddressETH", "WalletAddressTron", "WalletId", "WalletScanURL" },
                values: new object[,]
                {
                    { 1, 0m, "0107932b30922231adff71b4b7c0b05bc948632f56c2b62f98bd18fefeae8a9e", "0107932b30922231adff71b4b7c0b05bc948632f56c2b62f98bd18fefeae8a9e", 0m, 0m, 0m, 0m, 1, "0x31c1fe443E54d007FD1c8c5E7ae7C2356b374616", "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N", 1, "" },
                    { 2, 0m, "f7753fbb6a94a3f5758acfd83e2c568899220f2ba782b831b14ea5bfc95bc422", "0107932b30922231adff71b4b7c0b05bc948632f56c2b62f98bd18fefeae8a9e", 0m, 0m, 0m, 0m, 2, "0x09Dd4927885EdbC5Ad820Fe489d7409A58ebe6DA", "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N", 2, "" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_WalletDetailModels_WalletId",
                table: "WalletDetailModels",
                column: "WalletId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminLoginModels");

            migrationBuilder.DropTable(
                name: "Networks");

            migrationBuilder.DropTable(
                name: "TransferHistoryModels");

            migrationBuilder.DropTable(
                name: "userLoginModels");

            migrationBuilder.DropTable(
                name: "UserSignUpModels");

            migrationBuilder.DropTable(
                name: "WalletDetailModels");

            migrationBuilder.DropTable(
                name: "WalletModels");
        }
    }
}
