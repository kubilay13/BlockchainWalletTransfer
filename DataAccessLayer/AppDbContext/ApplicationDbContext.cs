using Entities.Models;
using Entities.Models.EthModels;
using Entities.Models.TronModels;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.AppDbContext
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<TronWalletModel> TronWalletModels { get; set; }
        public DbSet<TransferHistoryModel> TransferHistoryModels { get; set; }
        public DbSet<TransactionSuccesHistoryModel> TransactionSuccesHistoryModels { get; set; }
        public DbSet<Network> Networks { get; set; }
        public DbSet<EthWalletModels> EthWalletModelss { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TronWalletModel>().HasKey(t => t.Id);
            modelBuilder.Entity<TransferHistoryModel>().HasKey(t => t.Id);
            modelBuilder.Entity<TransactionSuccesHistoryModel>().HasKey(t => t.Id);

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TronWalletModel>().HasData(
                new TronWalletModel
                {
                    Id = 1,
                    WalletName = "TestAdress",
                    PrivateKeyTron = "5a87ccab1b8b8f2d86c24ad6f278d8030be5a17d056588242ef377d9c3ddeb8e",
                    WalletAddressTron = "TXTVwsUMsWrWsvd61VRcE9Bsk4WbEY9DGv",
                    CreatedAt = DateTime.UtcNow,
                    LastTransactionAt = DateTime.UtcNow,
                    LastTransactionTime = "10:49:03",
                    TrxAmount = 0,
                    UsdtAmount = 0,
                    UsdcAmount = 0,
                    ETHAmount = 0,
                    Network = "Testnet(Nile)",
                    WalletTronScanURL = "https://nile.tronscan.org/#/address/TXTVwsUMsWrWsvd61VRcE9Bsk4WbEY9DGv",
                    TransactionLimit = false,
                },
                 new TronWalletModel
                 {
                     Id = 2,
                     WalletName = "AdminAdress",
                     PrivateKeyTron = "0107932b30922231adff71b4b7c0b05bc948632f56c2b62f98bd18fefeae8a9e",
                     WalletAddressTron = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N",
                     CreatedAt = DateTime.UtcNow,
                     LastTransactionAt = DateTime.UtcNow,
                     LastTransactionTime = "10:49:03",
                     TrxAmount = 0,
                     UsdtAmount = 0,
                     UsdcAmount = 0,
                     ETHAmount = 0,
                     Network = "Testnet(Nile)",
                     WalletTronScanURL = "https://nile.tronscan.org/#/address/TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N",
                     TransactionLimit = true,
                 }
                );


            modelBuilder.Entity<Network>().HasData(
                new Network
                {
                    Id = 1,
                    Type = 0,
                    Networks = "TRON",
                    Name = "TRX",
                    Contract = null,
                    Decimal = 6,
                    Commission = 10,
                    AdminWallet = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N",
                    AdminWalletPrivateKey = "0107932b30922231adff71b4b7c0b05bc948632f56c2b62f98bd18fefeae8a9e",
                },
                 new Network
                 {
                     Id = 2,
                     Type = 0,
                     Networks = "TRON",
                     Name = "USDT",
                     Contract = "TXYZopYRdj2D9XRtbG411XZZ3kM5VkAeBf",
                     Decimal = 6,
                     Commission = 10,
                     AdminWallet = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N",
                     AdminWalletPrivateKey = "0107932b30922231adff71b4b7c0b05bc948632f56c2b62f98bd18fefeae8a9e",
                 }, new Network
                 {
                     Id = 3,
                     Type = 0,
                     Networks = "TRON",
                     Name = "USDC",
                     Contract = "TEMVynQpntMqkPxP6wXTW2K7e4sM3cRmWz",
                     Decimal = 6,
                     Commission = 10,
                     AdminWallet = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N",
                     AdminWalletPrivateKey = "0107932b30922231adff71b4b7c0b05bc948632f56c2b62f98bd18fefeae8a9e",
                 }
                );
            //TronWalletModel--

            modelBuilder.Entity<TronWalletModel>()
            .Property(t => t.PrivateKeyTron)
            .HasMaxLength(128);

            modelBuilder.Entity<TronWalletModel>()
                .Property(t => t.WalletAddressTron)
                .HasMaxLength(34)
                .IsRequired();

            modelBuilder.Entity<TronWalletModel>()
                .Property(t => t.CreatedAt)
                .HasColumnType("datetime2");

            modelBuilder.Entity<TronWalletModel>()
                .Property(t => t.CreatedAtTime)
                .HasMaxLength(8);

            modelBuilder.Entity<TronWalletModel>()
                .Property(t => t.LastTransactionAt)
                .HasColumnType("datetime2");

            modelBuilder.Entity<TronWalletModel>()
                .Property(t => t.LastTransactionTime)
                .HasMaxLength(8);

            modelBuilder.Entity<TronWalletModel>()
                .Property(t => t.TrxAmount)
                .HasColumnType("decimal(18, 8)");

            modelBuilder.Entity<TronWalletModel>()
               .Property(t => t.UsdtAmount)
               .HasColumnType("decimal(18, 8)");

            modelBuilder.Entity<TronWalletModel>()
               .Property(t => t.UsdcAmount)
               .HasColumnType("decimal(18, 8)");

            modelBuilder.Entity<TronWalletModel>()
             .Property(t => t.ETHAmount)
             .HasColumnType("decimal(18, 8)");

            modelBuilder.Entity<TronWalletModel>()
                .Property(t => t.WalletTronScanURL)
                .HasMaxLength(255);

            modelBuilder.Entity<TronWalletModel>()
                .Property(t => t.TransactionLimit)
                .IsRequired();

            //TransferHistoryModel--

            modelBuilder.Entity<TransferHistoryModel>()
                .Property(t => t.SendingAddress)
                .HasMaxLength(34)
                .IsRequired();

            modelBuilder.Entity<TransferHistoryModel>()
                .Property(t => t.ReceivedAddress)
                .HasMaxLength(34)
                .IsRequired();

            modelBuilder.Entity<TransferHistoryModel>()
                .Property(t => t.TransactionHash)
                .HasMaxLength(64)
                .IsRequired();

            modelBuilder.Entity<TransferHistoryModel>()
                .Property(t => t.CoinType)
                .HasMaxLength(10);

            modelBuilder.Entity<TransferHistoryModel>()
                .Property(t => t.TransactionNetwork)
                .HasMaxLength(10);

            modelBuilder.Entity<TransferHistoryModel>()
                .Property(t => t.TransactionDate)
                .HasColumnType("datetime2");

            modelBuilder.Entity<TransferHistoryModel>()
                .Property(t => t.TransactionDateTime)
                .HasMaxLength(8);

            modelBuilder.Entity<TransferHistoryModel>()
                .Property(t => t.Commission)
                .HasColumnType("decimal(18, 8)");

            modelBuilder.Entity<TransferHistoryModel>()
                .Property(t => t.NetworkFee)
                .HasColumnType("decimal(18, 8)");

            modelBuilder.Entity<TransferHistoryModel>()
                .Property(t => t.TransactionStatus)
                .IsRequired();
            modelBuilder.Entity<TronWalletModel>()
              .Property(t => t.WalletName)
              .HasMaxLength(100);

            //TransactionSuccesHistoryModel--

            modelBuilder.Entity<TransactionSuccesHistoryModel>()
            .Property(t => t.SendingAddress)
            .HasMaxLength(34)
            .IsRequired();

            modelBuilder.Entity<TransactionSuccesHistoryModel>()
                .Property(t => t.ReceivedAddress)
                .HasMaxLength(34)
                .IsRequired();

            modelBuilder.Entity<TransactionSuccesHistoryModel>()
                .Property(t => t.TransactionHash)
                .HasMaxLength(64)
                .IsRequired();

            modelBuilder.Entity<TransactionSuccesHistoryModel>()
                .Property(t => t.CoinType)
                .HasMaxLength(10);

            modelBuilder.Entity<TransactionSuccesHistoryModel>()
                .Property(t => t.TransactionNetwork)
                .HasMaxLength(10);

            modelBuilder.Entity<TransactionSuccesHistoryModel>()
                .Property(t => t.TransactionDate)
                .HasColumnType("datetime2");

            modelBuilder.Entity<TransactionSuccesHistoryModel>()
                .Property(t => t.TransactionDateTime)
                .HasMaxLength(8);

            modelBuilder.Entity<TransactionSuccesHistoryModel>()
                .Property(t => t.Commission)
                .HasColumnType("decimal(18, 8)");

            modelBuilder.Entity<TransactionSuccesHistoryModel>()
                .Property(t => t.NetworkFee)
                .HasColumnType("decimal(18, 8)");

            modelBuilder.Entity<TransactionSuccesHistoryModel>()
                .Property(t => t.TransactionStatus)
                .IsRequired();
        }
    }
}

