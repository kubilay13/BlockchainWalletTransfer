﻿using Business.Models.TronModels;
using Microsoft.EntityFrameworkCore;
using TronWalletApi.Models;
namespace TronWalletApi.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<TronWalletModel> TronWalletModels { get; set; }
        public DbSet<TransferHistoryModel> TransferHistoryModels { get; set; }
        public DbSet<TransactionSuccesHistoryModel> TransactionSuccesHistoryModels { get; set; }
        public DbSet<Network> Networks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TronWalletModel>().HasKey(t => t.Id);
            modelBuilder.Entity<TransferHistoryModel>().HasKey(t => t.Id);
            modelBuilder.Entity<TransactionSuccesHistoryModel>().HasKey(t => t.Id);

            //TronWalletModel--

            modelBuilder.Entity<TronWalletModel>()
                .Property(t => t.PrivateKey)
                .HasMaxLength(128);

            modelBuilder.Entity<TronWalletModel>()
                .Property(t => t.WalletAddress)
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


