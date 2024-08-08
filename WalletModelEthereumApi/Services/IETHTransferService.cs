namespace WalletModelEthereumApi.Services
{
    public interface IETHTransferService
    {
        Task<string> SenTransactionEthAsync();
    }
}
