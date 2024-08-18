using Entities.Models.EthModels;
using Entities.Models;
using Org.BouncyCastle.Asn1.Ocsp;
using Entities.Models.TronModels;
namespace ETHWalletApi.Services
{
    public interface IEthService
    {
        Task<WalletModel> CreateETHWalletAsync(string walletName);
        Task<string> GetPrivateKeyByAddressAsync(string walletAddress);
        Task<string> SendTransactionAsyncETH(TransferRequest request);
        Task<string> SendTransactionAsyncUSDT(EthNetworkTransactionRequest request);

    }
}
