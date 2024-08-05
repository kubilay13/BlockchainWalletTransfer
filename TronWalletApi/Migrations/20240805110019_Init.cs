using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

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
                name: "TransactionErrorHistoryModels",
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
                    TransferFee = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
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
                    table.PrimaryKey("PK_TransactionErrorHistoryModels", x => x.Id);
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
                    TransferFee = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
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
                name: "TronWalletDepositModels",
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
                    TransferFee = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
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
                    table.PrimaryKey("PK_TronWalletDepositModels", x => x.Id);
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
                    Network = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WalletTronScanURL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TransactionLimit = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TronWalletModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TronWalletWithdrawModels",
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
                    TransferFee = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
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
                    table.PrimaryKey("PK_TronWalletWithdrawModels", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Networks");

            migrationBuilder.DropTable(
                name: "TransactionErrorHistoryModels");

            migrationBuilder.DropTable(
                name: "TransactionSuccesHistoryModels");

            migrationBuilder.DropTable(
                name: "TransferHistoryModels");

            migrationBuilder.DropTable(
                name: "TronWalletDepositModels");

            migrationBuilder.DropTable(
                name: "TronWalletModels");

            migrationBuilder.DropTable(
                name: "TronWalletWithdrawModels");
        }
    }
}
