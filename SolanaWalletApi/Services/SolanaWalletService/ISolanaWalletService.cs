namespace SolanaWalletApi.Services.SolanaWalletService
{
    public interface ISolanaWalletService
    {
        Task<string> CreateSolanaWalletToSign(string WalletName);
    }
}
