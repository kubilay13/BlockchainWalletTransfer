﻿// <auto-generated />
using System;
using DataAccessLayer.AppDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace TronWalletApi.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Entities.Models.Network", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AdminWallet")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AdminWalletPrivateKey")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Commission")
                        .HasColumnType("decimal(18,8)");

                    b.Property<string>("Contract")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Decimal")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NetworkId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Networks")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Networks");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AdminWallet = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N",
                            AdminWalletPrivateKey = "0107932b30922231adff71b4b7c0b05bc948632f56c2b62f98bd18fefeae8a9e",
                            Commission = 10m,
                            Decimal = 6,
                            Name = "TRX",
                            Networks = "TRON",
                            Type = 0
                        },
                        new
                        {
                            Id = 2,
                            AdminWallet = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N",
                            AdminWalletPrivateKey = "0107932b30922231adff71b4b7c0b05bc948632f56c2b62f98bd18fefeae8a9e",
                            Commission = 10m,
                            Contract = "TXYZopYRdj2D9XRtbG411XZZ3kM5VkAeBf",
                            Decimal = 6,
                            Name = "USDT",
                            Networks = "TRON",
                            Type = 0
                        },
                        new
                        {
                            Id = 3,
                            AdminWallet = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N",
                            AdminWalletPrivateKey = "0107932b30922231adff71b4b7c0b05bc948632f56c2b62f98bd18fefeae8a9e",
                            Commission = 10m,
                            Contract = "TEMVynQpntMqkPxP6wXTW2K7e4sM3cRmWz",
                            Decimal = 6,
                            Name = "USDC",
                            Networks = "TRON",
                            Type = 0
                        });
                });

            modelBuilder.Entity("Entities.Models.TronModels.TransactionSuccesHistoryModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CoinType")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<decimal>("Commission")
                        .HasColumnType("decimal(18, 8)");

                    b.Property<string>("Network")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("NetworkFee")
                        .HasColumnType("decimal(18, 8)");

                    b.Property<string>("ReceivedAddress")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("ReceiverTransactionUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SenderTransactionUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SendingAddress")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<decimal>("TransactionAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("TransactionDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("TransactionDateTime")
                        .HasMaxLength(8)
                        .HasColumnType("nvarchar(8)");

                    b.Property<string>("TransactionHash")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("TransactionNetwork")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<bool>("TransactionStatus")
                        .HasColumnType("bit");

                    b.Property<int>("TransactionType")
                        .HasColumnType("int");

                    b.Property<string>("TransactionUrl")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TransactionSuccesHistoryModels");
                });

            modelBuilder.Entity("Entities.Models.TronModels.TransferHistoryModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CoinType")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<decimal>("Commission")
                        .HasColumnType("decimal(18, 8)");

                    b.Property<string>("Network")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("NetworkFee")
                        .HasColumnType("decimal(18, 8)");

                    b.Property<string>("ReceivedAddress")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("SendingAddress")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<decimal>("TransactionAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("TransactionDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("TransactionHash")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("TransactionNetwork")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<bool>("TransactionStatus")
                        .HasColumnType("bit");

                    b.Property<int>("TransactionType")
                        .HasColumnType("int");

                    b.Property<string>("TransactionUrl")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TransferHistoryModels");
                });

            modelBuilder.Entity("Entities.Models.TronModels.WalletModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastTransactionAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Network")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TransactionLimit")
                        .HasColumnType("bit");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("WalletName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("WalletScanURL")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("WalletModels");
                });

            modelBuilder.Entity("Entities.Models.WalletDetailModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("ETHAmount")
                        .HasColumnType("decimal(18, 8)");

                    b.Property<string>("PrivateKeyEth")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PrivateKeyTron")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("PublicKeyEth")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("TrxAmount")
                        .HasColumnType("decimal(18, 8)");

                    b.Property<decimal>("UsdcAmount")
                        .HasColumnType("decimal(18, 8)");

                    b.Property<decimal>("UsdtAmount")
                        .HasColumnType("decimal(18, 8)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("WalletAddressETH")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WalletAddressTron")
                        .IsRequired()
                        .HasMaxLength(34)
                        .HasColumnType("nvarchar(34)");

                    b.Property<int>("WalletId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("WalletId");

                    b.ToTable("WalletDetailModels");
                });

            modelBuilder.Entity("Entities.Models.WalletDetailModel", b =>
                {
                    b.HasOne("Entities.Models.TronModels.WalletModel", "Wallet")
                        .WithMany("WalletDetails")
                        .HasForeignKey("WalletId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Wallet");
                });

            modelBuilder.Entity("Entities.Models.TronModels.WalletModel", b =>
                {
                    b.Navigation("WalletDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
