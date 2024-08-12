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
        public EthService(string nodeUrl, string privateKey)
        {
            var account = new Nethereum.Web3.Accounts.Account(privateKey); 
            _web3 = new Web3(account, nodeUrl);
        }
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
            var web3 = new Web3(account);

            var amountInWei = Web3.Convert.ToWei(request.Amount.Value);
            var gasPrice = new HexBigInteger(Web3.Convert.ToWei(2, UnitConversion.EthUnit.Gwei));
            var gasLimit = new HexBigInteger(21000);

            try
            {
                var transaction = new TransactionInput
                {
                    From = request.FromAddress,
                    To = request.ToAddress,
                    Value = new HexBigInteger(amountInWei),
                    GasPrice = gasPrice,
                    Gas = gasLimit
                };

                var txnHash = await web3.Eth.Transactions.SendTransaction.SendRequestAsync(transaction);
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
