using ETHWalletApi.Models;

namespace ETHWalletApi.Services
{
    public interface IEthService
    {
        Task<string> SendTransactionAsyncs(EthNetworkTransactionRequest eTHWalletModels);
    }
}
