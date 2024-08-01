using TronWalletApi.Models;

public interface ITronService
{
    Task<string> CreateWallet(string walletName);
    Task<decimal> GetBalanceAsync(string address);
    Task<decimal> GetBalanceAsyncUsdt(string address,string privatekey);
    Task<decimal> GetTronUsdPriceAsync();
    Task<decimal> GetBalance(string address);
    Task SendTronAsync(string senderAddress, string receiverAddress, long amount);
    Task Transfer(TransferRequest request);
    Task<TransactionInfoModel> GetTransactionFeeAsync(string transactionHash);
    Task UsdtTransfer(TransferRequest request);
}
