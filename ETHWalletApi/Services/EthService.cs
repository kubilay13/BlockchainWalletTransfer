using ETHWalletApi.Models;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.Model;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Signer;
using Nethereum.Util;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace ETHWalletApi.Services
{
    public class EthService : IEthService
    {
        private readonly Web3 _web3;

        public EthService( )
        {
            _web3 = new Web3("https://sepolia.infura.io/v3/3fcb68529b9e4288a4eb599f266bbb50");
        }

        //public EthService(string nodeUrl, string privateKey)
        //{
        //    var account = new Nethereum.Web3.Accounts.Account(privateKey); 
        //    _web3 = new Web3(account, nodeUrl);
        //}
        public async Task<EthWalletModels> CreateETHWalletAsync(string walletName)
        {
            var EthKey = EthECKey.GenerateKey();
            var privateKey = EthKey.GetPrivateKeyAsBytes().ToHex();
            var publicKey = EthKey.GetPubKey().ToHex();
            var address = EthKey.GetPublicAddress();

            if (privateKey == null || publicKey == null || address == null)
            {
                throw new ApplicationException("Cüzdan Oluşturma İşlemi Başarısız.");
            }else
            {
                var walletDetails = new EthWalletModels
                {
                    WalletName = walletName,
                    PrivateKey = privateKey,
                    PublicKey = publicKey,
                    WalletAddress = address,
                    ETHAmount = 0,
                    Network = "ETH",
                    WalletETHScanURL = $"https://etherscan.io/address/{address}"
                };
                return walletDetails;
            }
        }

        public async Task<string> SendTransactionAsync(EthNetworkTransactionRequest request)
        {
            var account = new Nethereum.Web3.Accounts.Account(request.PrivateKey);
            var web3 = new Web3(account,_web3.Client);

            var amountInWei = Web3.Convert.ToWei(request.Amount.Value);
            var gasPrice = new HexBigInteger(Web3.Convert.ToWei(25, UnitConversion.EthUnit.Gwei));
            var gasLimit = new HexBigInteger(21000);
            var _gas = await web3.Eth.GasPrice.SendRequestAsync();
            var currentNonce = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(account.Address);
            try
            {
                var transaction = new TransactionInput
                {
                    From = request.FromAddress,
                    To = request.ToAddress,
                    Value = new HexBigInteger(amountInWei),
                    GasPrice = _gas,
                    Nonce = currentNonce,
                    //Gas = gasLimit
                };
                var signature= await web3.TransactionManager.SignTransactionAsync(transaction);
                var txnHash = await web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(signature);

                return txnHash;
            }
            catch (RpcResponseException ex)
            {
                throw new InvalidOperationException($"Transaction failed with RPC error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Transaction failed due to an unexpected error", ex);
            }
        }
    }
}
