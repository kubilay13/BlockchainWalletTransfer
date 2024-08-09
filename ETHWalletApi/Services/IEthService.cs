namespace ETHWalletApi.Services
{
    public interface IEthService
    {
        Task<string> SendTransactionAsync(string fromAddress, string toAddress, decimal amount, string privateKey);
    }
}
