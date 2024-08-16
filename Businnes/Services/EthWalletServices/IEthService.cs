using Entities.Models.EthModels;
using Entities.Models;
using Org.BouncyCastle.Asn1.Ocsp;
namespace ETHWalletApi.Services
{
    public interface IEthService
    {
        Task<EthWalletModels> CreateETHWalletAsync(string walletName);
        Task<string> GetPrivateKeyByAddressAsync(string walletAddress);
        Task<string> SendTransactionAsyncETH(EthNetworkTransactionRequest request);

        Task<string> SendTransactionAsyncUSDT(EthNetworkTransactionRequest request);

    }
}
