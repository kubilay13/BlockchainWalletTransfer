using ETHWalletApi.Models;
using Org.BouncyCastle.Asn1.Ocsp;

namespace ETHWalletApi.Services
{
    public interface IEthService
    {
        Task<string> SendTransactionAsync(EthNetworkTransactionRequest request);
        Task<EthWalletModels> CreateETHWalletAsync(string walletName );

    }
}
