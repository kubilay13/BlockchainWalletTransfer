using Nethereum;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using Nethereum.Web3;
namespace ETHWalletApi.Services
{
    public class EthService
    {
        private readonly IWeb3 _web3;

        public EthService(string NodeUrl)
        {
            _web3 = new Web3(NodeUrl);
        }
        public async Task<string> SendTransactionAsync(string fromAddress, string toAddress, decimal amount, string privateKey)
        {
            var transaction = new TransactionInput
            {
                From = fromAddress,
                To = toAddress,
                //Value=Web3.Convert.ToWei(amount)
            };
            var gasPrice = Web3.Convert.ToWei(20, UnitConversion.EthUnit.Gwei);
            var gasLimit = new HexBigInteger(21000);

            var transactionHash = await _web3.Eth.Transactions.SendTransaction.SendRequestAsync(transaction);
            return transactionHash;
        }
    }
}
