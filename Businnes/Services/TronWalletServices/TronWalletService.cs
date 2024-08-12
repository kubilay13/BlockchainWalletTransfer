using Business.Services.TronService;
using DataAccessLayer.AppDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace TronWalletApi.Services.TronWalletService
{
    public class TronWalletService : ITronWalletService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ITronService _tronService;
        private readonly ILogger<TronWalletService> _logger;

        public TronWalletService(ITronService tronService, ApplicationDbContext applicationDbContext, ILogger<TronWalletService> logger)
        {
            _tronService = tronService;
            _applicationDbContext = applicationDbContext;
            _logger = logger;
        }

        public async Task GetNetworkFee()
        {
            var transferHistories = await _applicationDbContext.TransferHistoryModels
                .Where(th => th.NetworkFee == 0)
                .ToListAsync();

            foreach (var transferHistory in transferHistories)
            {
                bool feeUpdated = false;

                while (!feeUpdated)
                {
                    try
                    {
                        var transactionFee = await _tronService.GetTransactionFeeAsync(transferHistory.TransactionHash!);

                        if (transactionFee.Receipt != null)
                        {
                            if (transactionFee?.Fee != null)
                            {

                                transferHistory.NetworkFee = transactionFee.Fee / 1000000;

                                _applicationDbContext.TransferHistoryModels.Update(transferHistory);
                                await _applicationDbContext.SaveChangesAsync();
                                feeUpdated = true;
                            }
                            else
                            {
                                _logger.LogWarning($"NetUsage is non-positive: {transactionFee.Receipt.NetUsage}");
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Receipt is null in transactionFee.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"İşlem ücreti güncellenirken bir hata oluştu. TransactionHash: {transferHistory.TransactionHash}, Hata: {ex.Message}");
                        await Task.Delay(TimeSpan.FromSeconds(10));
                    }
                }
            }
        }

        public async Task UpdateWalletAmountsAsync()
        {
            _logger.LogInformation("Wallet miktarları güncelleniyor.");

            try
            {

                var wallets = await _applicationDbContext.TronWalletModels.ToListAsync();

                if (wallets != null)
                {
                    foreach (var wallet in wallets)
                    {
                        try
                        {
                            var balance = await _tronService.GetBalanceAsync(wallet.WalletAddress!);
                            wallet.TrxAmount = balance;

                            var UsdtBalance = await _tronService.GetBalanceAsyncUsdt(wallet.WalletAddress, wallet.PrivateKey);
                            wallet.UsdtAmount = UsdtBalance;

                            var UsdcBalance = await _tronService.GetBalanceAsyncUsdc(wallet.WalletAddress, wallet.PrivateKey);
                            wallet.UsdcAmount = UsdcBalance;

                            var transferHistories = await _applicationDbContext.TransferHistoryModels
                                .Where(th => th.NetworkFee == 0)
                                .ToListAsync();

                            foreach (var transferHistory in transferHistories)
                            {

                                try
                                {
                                    var transactionFee = await _tronService.GetTransactionFeeAsync(transferHistory.TransactionHash!);

                                    if (transactionFee.Receipt != null)
                                    {
                                        if (transactionFee?.Fee != null)
                                        {
                                            transferHistory.NetworkFee = transactionFee.Fee / 1000000;
                                            await _applicationDbContext.SaveChangesAsync();

                                            _logger.LogInformation("Transfer Ücreti Güncellendi");
                                        }
                                        else
                                        {
                                            _logger.LogWarning($"NetUsage is non-positive: {transactionFee.Receipt.NetUsage}");
                                        }
                                    }
                                    else
                                    {
                                        _logger.LogWarning("Receipt is null in transactionFee.");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError($"İşlem ücreti güncellenirken bir hata oluştu. TransactionHash: {transferHistory.TransactionHash}, Hata: {ex.Message}");
                                    await Task.Delay(TimeSpan.FromSeconds(10));
                                }
                            }


                            _applicationDbContext.TronWalletModels.Update(wallet);
                            await _applicationDbContext.SaveChangesAsync();
                            _logger.LogInformation("Cüzdan bakiyesi ve NetworkFee başarıyla güncellendi.");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Cüzdan bakiyesi güncellenirken bir hata oluştu. Cüzdan Adresi: {wallet.WalletAddress}");
                        }
                    }

                    _logger.LogInformation("Wallet miktarları başarıyla güncellendi.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Wallet miktarları güncellenirken bir hata oluştu.");
            }
        }

    }
}
