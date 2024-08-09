using ETHWalletApi.Models;
using Nethereum;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System.Numerics;

namespace ETHWalletApi.Services
{
    public class EthService : IEthService
    {
        private readonly Web3 _web3;

        public EthService(string nodeUrl)
        {
            // Web3 örneğini nodeUrl ile başlatıyoruz
            _web3 = new Web3(nodeUrl);
        }

        public async Task<string> SendTransactionAsyncs(EthNetworkTransactionRequest request)
        {
            var account = new Account(request.PrivateKey);
            var amountInWei = Web3.Convert.ToWei(request.Amount.Value);
            var transactionInput = new TransactionInput
            {
                From = request.FromAddress,
                To = request.ToAddress,
                Value = new HexBigInteger(amountInWei),
                GasPrice = new HexBigInteger(Web3.Convert.ToWei(2, UnitConversion.EthUnit.Gwei)), // Sabit bir değer
                Gas = new HexBigInteger(21000) // Sabit bir değer
            };
            var web3 = new Web3("https://sepolia.infura.io/v3/3fcb68529b9e4288a4eb599f266bbb50");
            var blockNumber = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
            Console.WriteLine($"Current Block Number: {blockNumber.Value}");

            try
            {
                var txnHash = await _web3.Eth.Transactions.SendTransaction.SendRequestAsync(transactionInput);
                return txnHash;
            }
            catch (RpcResponseException ex)
            {
                // RPC hata mesajını döndür
                throw new InvalidOperationException($"Transaction failed with RPC error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Diğer istisnaları yakala
                throw new InvalidOperationException("Transaction failed due to an unexpected error", ex);
            }
        }
    }
}
