using Business.Models.TronModels;
using TronWalletApi.Models;

public interface ITronService
{
    Task<string> CreateWallet(string walletName);
    Task<decimal> GetBalanceAsync(string address);
    Task<decimal> GetBalanceAsyncUsdt(string address, string privatekey);
    Task<decimal> GetBalanceAsyncUsdc(string UsdcBalance, string privatekey);
    Task<decimal> GetTronUsdApiPriceAsync();
    Task<decimal> GetBalanceAsyncTrx(string address);
    Task SendTronAsync(string senderAddress, string receiverAddress, long amount);
    Task TransferTRXorToken(TransferRequest request);
    Task<TransactionInfoModel> GetTransactionFeeAsync(string transactionHash);
    Task TokenTransfer(TransferRequest request);
}
