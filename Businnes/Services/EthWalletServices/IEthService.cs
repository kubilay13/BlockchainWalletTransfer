using Entities.Models.EthModels;
using Entities.Models;
using Org.BouncyCastle.Asn1.Ocsp;
namespace ETHWalletApi.Services
{
    public interface IEthService
    {
        Task<EthWalletModels> CreateETHWalletAsync(string walletName);
        Task<string> SendTransactionAsync(EthNetworkTransactionRequest request);
    }
}
