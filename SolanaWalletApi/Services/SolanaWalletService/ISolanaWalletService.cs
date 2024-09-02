using Entities.Dto.WalletApiDto;

namespace SolanaWalletApi.Services.SolanaWalletService
{
    public interface ISolanaWalletService
    {
        Task<string> CreateSolanaWalletToSign(string WalletName);
        Task<string> SendTransactionSolanaAsync(TransferRequest request);
    }
}
