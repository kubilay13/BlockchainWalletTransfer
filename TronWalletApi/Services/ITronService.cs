using TronWalletApi.Models;

public interface ITronService
{
    Task<string> CreateWallet(string walletName);
    Task<decimal> GetBalanceAsync(string address);
    Task<decimal> GetTronUsdPriceAsync();
    Task<decimal> GetBalance(string address);
    Task SendTronAsync(string senderAddress, string receiverAddress, long amount);
    Task Transfer(TransferRequest request);
    //Task<string> SendUsdt(string senderAddress, string receiveAddress, decimal amount, bool isDeposit);
   
    Task<TransactionInfoModel> GetTransactionFeeAsync(string transactionHash);
    Task UsdtTransfer(TransferRequest request);
}
