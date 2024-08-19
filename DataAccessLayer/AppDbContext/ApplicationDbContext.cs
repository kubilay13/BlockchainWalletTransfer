using Entities.Models.AdminModel;
using Entities.Models.NetworkModel;
using Entities.Models.TronModels;
using Entities.Models.WalletModel;
using Microsoft.EntityFrameworkCore;
namespace DataAccessLayer.AppDbContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<WalletModel> WalletModels { get; set; }
        public DbSet<TransferHistoryModel> TransferHistoryModels { get; set; }
        public DbSet<Network> Networks { get; set; }
        public DbSet<WalletDetailModel> WalletDetailModels { get; set; }
        public DbSet<AdminLoginModel> AdminLoginModels { get; set; }

        //public DbSet<UserSignUpModel> UserSignUpModels { get; set; }

        //public DbSet<UserLoginModel> userLoginModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WalletModel>().HasKey(t => t.Id);
            modelBuilder.Entity<TransferHistoryModel>().HasKey(t => t.Id);
            modelBuilder.Entity<WalletModel>().HasMany(w => w.WalletDetails).WithOne(wd => wd.Wallet).HasForeignKey(wd => wd.WalletId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<WalletDetailModel>().HasOne(wd => wd.Wallet).WithMany(w => w.WalletDetails).HasForeignKey(wd => wd.WalletId);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WalletModel>().HasData(
                new WalletModel
                {
                    Id = 1,
                    UserId = 0,
                    Name = "TRXAdminAdress",
                    Surname = "SurnameAdminTRX",
                    Email = "user@example.com",
                    TelNo = "stringstri",
                    Password = "Password",
                    WalletName = "TRXAdminAdress",
                    CreatedAt = DateTime.UtcNow,
                    LastTransactionAt = DateTime.UtcNow,
                    TransactionLimit = true,
                    Network = "Testnet(Nile)",
                });



            modelBuilder.Entity<WalletDetailModel>().HasData(
                new WalletDetailModel
                {
                    Id = 1,
                    UserId = 1,
                    WalletAddressTron = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N",
                    PrivateKeyTron = "0107932b30922231adff71b4b7c0b05bc948632f56c2b62f98bd18fefeae8a9e",
                    TrxAmount = 0,
                    UsdtAmount = 0,
                    UsdcAmount = 0,
                    WalletAddressETH = "0x31c1fe443E54d007FD1c8c5E7ae7C2356b374616",
                    PrivateKeyEth = "0107932b30922231adff71b4b7c0b05bc948632f56c2b62f98bd18fefeae8a9e",
                    ETHAmount = 0,
                    WalletScanURL = "",
                    WalletId = 1,

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

            modelBuilder.Entity<WalletDetailModel>()
            .Property(t => t.PrivateKeyTron)
            .HasMaxLength(128);

            modelBuilder.Entity<WalletDetailModel>()
                .Property(t => t.WalletAddressTron)
                .HasMaxLength(34)
                .IsRequired();

            modelBuilder.Entity<WalletModel>()
                .Property(t => t.CreatedAt)
                .HasColumnType("datetime2");

            modelBuilder.Entity<WalletModel>()
                .Property(t => t.LastTransactionAt)
                .HasColumnType("datetime2");

            modelBuilder.Entity<WalletDetailModel>()
                .Property(t => t.TrxAmount)
                .HasColumnType("decimal(18, 8)");

            modelBuilder.Entity<WalletDetailModel>()
               .Property(t => t.UsdtAmount)
               .HasColumnType("decimal(18, 8)");

            modelBuilder.Entity<WalletDetailModel>()
               .Property(t => t.UsdcAmount)
               .HasColumnType("decimal(18, 8)");

            modelBuilder.Entity<WalletDetailModel>()
             .Property(t => t.ETHAmount)
             .HasColumnType("decimal(18, 8)");

            modelBuilder.Entity<WalletDetailModel>()
                .Property(t => t.WalletScanURL)
                .HasMaxLength(255);

            modelBuilder.Entity<WalletModel>()
                .Property(t => t.TransactionLimit)
                .IsRequired();

            //TransferHistoryModel--

            modelBuilder.Entity<TransferHistoryModel>()
                .Property(t => t.SendingAddress)
                .HasMaxLength(64)
                .IsRequired();

            modelBuilder.Entity<TransferHistoryModel>()
                .Property(t => t.ReceivedAddress)
                .HasMaxLength(64)
                .IsRequired();

            modelBuilder.Entity<TransferHistoryModel>()
                .Property(t => t.TransactionHash)
                .HasMaxLength(256)
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
                .Property(t => t.Commission)
                .HasColumnType("decimal(18, 8)");

            modelBuilder.Entity<TransferHistoryModel>()
                .Property(t => t.NetworkFee)
                .HasColumnType("decimal(18, 8)");

            modelBuilder.Entity<TransferHistoryModel>()
                .Property(t => t.TransactionStatus)
                .IsRequired();
            modelBuilder.Entity<WalletModel>()
              .Property(t => t.WalletName)
              .HasMaxLength(100);
        }
    }
}

