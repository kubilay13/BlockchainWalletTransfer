﻿// <auto-generated />
using System;
using DataAccessLayer.AppDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace TronWalletApi.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240830201042_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Entities.Models.AdminModel.AdminLoginModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AdminLoginModels");
                });

            modelBuilder.Entity("Entities.Models.NetworkModel.Network", b =>
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
                        },
                        new
                        {
                            Id = 4,
                            AdminWallet = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N",
                            AdminWalletPrivateKey = "0107932b30922231adff71b4b7c0b05bc948632f56c2b62f98bd18fefeae8a9e",
                            Commission = 10m,
                            Contract = "TFT7sNiNDGZcqL7z7dwXUPpxrx1Ewk8iGL",
                            Decimal = 18,
                            Name = "USDD",
                            Networks = "TRON",
                            Type = 0
                        },
                        new
                        {
                            Id = 5,
                            AdminWallet = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N",
                            AdminWalletPrivateKey = "0107932b30922231adff71b4b7c0b05bc948632f56c2b62f98bd18fefeae8a9e",
                            Commission = 10m,
                            Contract = "TNuoKL1ni8aoshfFL1ASca1Gou9RXwAzfn",
                            Decimal = 18,
                            Name = "BTT",
                            Networks = "TRON",
                            Type = 0
                        });
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

            modelBuilder.Entity("Entities.Models.UserModel.UserLoginModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserMailAdress")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("userLoginModels");
                });

            modelBuilder.Entity("Entities.Models.UserModel.UserSignUpModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AccountName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("TelNo")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<string>("WalletName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("UserSignUpModels");
                });

            modelBuilder.Entity("Entities.Models.WalletModel.WalletDetailModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("BttAmount")
                        .HasColumnType("decimal(18, 8)");

                    b.Property<decimal>("ETHAmount")
                        .HasColumnType("decimal(18, 8)");

                    b.Property<decimal>("ETHBnbAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("ETHUsdtAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("PrivateKeyEth")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PrivateKeyTron")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<decimal>("TrxAmount")
                        .HasColumnType("decimal(18, 8)");

                    b.Property<decimal>("UsdcAmount")
                        .HasColumnType("decimal(18, 8)");

                    b.Property<decimal>("UsddAmount")
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

                    b.Property<string>("WalletScanURL")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("WalletId");

                    b.ToTable("WalletDetailModels");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            BttAmount = 0m,
                            ETHAmount = 0m,
                            ETHBnbAmount = 0m,
                            ETHUsdtAmount = 0m,
                            PrivateKeyEth = "0107932b30922231adff71b4b7c0b05bc948632f56c2b62f98bd18fefeae8a9e",
                            PrivateKeyTron = "0107932b30922231adff71b4b7c0b05bc948632f56c2b62f98bd18fefeae8a9e",
                            TrxAmount = 0m,
                            UsdcAmount = 0m,
                            UsddAmount = 0m,
                            UsdtAmount = 0m,
                            UserId = 1,
                            WalletAddressETH = "0x31c1fe443E54d007FD1c8c5E7ae7C2356b374616",
                            WalletAddressTron = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N",
                            WalletId = 1,
                            WalletScanURL = ""
                        },
                        new
                        {
                            Id = 2,
                            BttAmount = 0m,
                            ETHAmount = 0m,
                            ETHBnbAmount = 0m,
                            ETHUsdtAmount = 0m,
                            PrivateKeyEth = "f7753fbb6a94a3f5758acfd83e2c568899220f2ba782b831b14ea5bfc95bc422",
                            PrivateKeyTron = "0107932b30922231adff71b4b7c0b05bc948632f56c2b62f98bd18fefeae8a9e",
                            TrxAmount = 0m,
                            UsdcAmount = 0m,
                            UsddAmount = 0m,
                            UsdtAmount = 0m,
                            UserId = 2,
                            WalletAddressETH = "0x09Dd4927885EdbC5Ad820Fe489d7409A58ebe6DA",
                            WalletAddressTron = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N",
                            WalletId = 2,
                            WalletScanURL = ""
                        });
                });

            modelBuilder.Entity("Entities.Models.WalletModel.WalletModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AccountName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("LastTransactionAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Network")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("TelNo")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<bool>("TransactionLimit")
                        .HasColumnType("bit");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("WalletName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("WalletModels");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatedAt = new DateTime(2024, 8, 30, 20, 10, 42, 191, DateTimeKind.Utc).AddTicks(1100),
                            Email = "user@example.com",
                            LastTransactionAt = new DateTime(2024, 8, 30, 20, 10, 42, 191, DateTimeKind.Utc).AddTicks(1104),
                            Name = "TRXAdminAdress",
                            Network = "Testnet(Nile)",
                            Password = "Password",
                            Surname = "SurnameAdminTRX",
                            TelNo = "stringstri",
                            TransactionLimit = true,
                            UserId = 0,
                            WalletName = "TRXAdminAdress"
                        },
                        new
                        {
                            Id = 2,
                            CreatedAt = new DateTime(2024, 8, 30, 20, 10, 42, 191, DateTimeKind.Utc).AddTicks(1107),
                            Email = "user@example.com",
                            LastTransactionAt = new DateTime(2024, 8, 30, 20, 10, 42, 191, DateTimeKind.Utc).AddTicks(1107),
                            Name = "ETHAdminAdress",
                            Network = "TestNet(Sepolia)",
                            Password = "Password",
                            Surname = "SurnameAdminTETH",
                            TelNo = "stringstri",
                            TransactionLimit = true,
                            UserId = 0,
                            WalletName = "ETHAdminAdress"
                        });
                });

            modelBuilder.Entity("Entities.Models.WalletModel.WalletDetailModel", b =>
                {
                    b.HasOne("Entities.Models.WalletModel.WalletModel", "Wallet")
                        .WithMany("WalletDetails")
                        .HasForeignKey("WalletId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Wallet");
                });

            modelBuilder.Entity("Entities.Models.WalletModel.WalletModel", b =>
                {
                    b.Navigation("WalletDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
