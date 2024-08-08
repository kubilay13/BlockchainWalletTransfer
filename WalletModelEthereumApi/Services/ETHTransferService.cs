using Nethereum.Web3;
using Nethereum;
namespace WalletModelEthereumApi.Services
{
    public class ETHTransferService
    {
        private readonly IWeb3 _web3;

        public ETHTransferService(string NodeUrl)
        {
            _web3 = new Web3(NodeUrl);
        }
        
    }
}
