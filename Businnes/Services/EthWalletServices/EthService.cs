using DataAccessLayer.AppDbContext;
using Entities.Models.EthModels;
using Entities.Models.TronModels;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Signer;
using Nethereum.Util;
using Nethereum.Web3;

namespace ETHWalletApi.Services
{
    public class EthService : IEthService
    {
        private readonly Web3 _web3;
        private readonly ApplicationDbContext _applicationDbContext;
        public EthService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            _web3 = new Web3("https://sepolia.infura.io/v3/3fcb68529b9e4288a4eb599f266bbb50");
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
            }
            else
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
                var EthSaveDbVallet = new EthWalletModels
                {
                    WalletName = walletName,
                    PrivateKey = privateKey,
                    PublicKey = publicKey,
                    WalletAddress = address,
                    ETHAmount = 0,
                    Network = "ETH",
                    WalletETHScanURL = $"https://etherscan.io/address/{address}"
                };
                _applicationDbContext.EthWalletModelss.Add(EthSaveDbVallet);
                await _applicationDbContext.SaveChangesAsync();
                return walletDetails;
            }
        }
        public async Task<string> SendTransactionAsync(EthNetworkTransactionRequest request)
        {
            var account = new Nethereum.Web3.Accounts.Account(request.PrivateKey);
            var web3 = new Web3(account, _web3.Client);
            var amountInWei = Web3.Convert.ToWei(request.Amount.Value);
            //var gasPrice = new HexBigInteger(Web3.Convert.ToWei(25, UnitConversion.EthUnit.Gwei));
            var _gas = await web3.Eth.GasPrice.SendRequestAsync();
            var currentNonce = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(account.Address);
            try
            {
                var transaction = new TransactionInput
                {
                    From = request.FromAddress,
                    To = request.ToAddress,
                    Value =new HexBigInteger( amountInWei),
                    Gas = _gas,
                    Nonce = currentNonce,
                };
                
                var signature = await web3.TransactionManager.SignTransactionAsync(transaction);
                var txnHash = await web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(signature);
                var EthTransaction = new TransferHistoryModel
                {
                    ReceivedAddress = request.FromAddress,
                    SendingAddress = request.ToAddress,
                    TransactionHash = txnHash,
                    CoinType ="ETH",
                    TransactionNetwork="ETH",
                    TransactionAmount=request.Amount.Value,
                    Commission =0,
                    NetworkFee= Convert.ToDecimal(_gas),
                    TransactionUrl=$"https://etherscan.io/tx/{txnHash}",
                    TransactionStatus=true,
                    

                };
                //_applicationDbContext.TransferHistoryModels.Add(transaction);
                //await _applicationDbContext.SaveChangesAsync();
                return txnHash;


            }
            catch (RpcResponseException ex)
            {
                throw new InvalidOperationException($"ETH Transfer İşlemi Başarısız: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("ETH Transfer İşleminde Beklenmeyen Bir Hata Oluştu. ", ex);
            }
        }

        //public async Task SendUSDTTransactionAsync()
        //{
        //    // Ethereum ağına bağlantı (Infura ya da başka bir sağlayıcı kullanabilirsiniz)
        //    var url = "https://mainnet.infura.io/v3/YOUR_INFURA_PROJECT_ID";
        //    var privateKey = "YOUR_PRIVATE_KEY";
        //    var accountAddress = "YOUR_ACCOUNT_ADDRESS";

        //    // USDT Sözleşme Adresi (Bu adresi kontrol ettiğinizden emin olun)
        //    var usdtContractAddress = "USDT_CONTRACT_ADDRESS";

        //    // ERC-20 ABI
        //    var abi = @"[
        //    {
        //      'constant': false,
        //      'inputs': [
        //        {
        //          'name': '_to',
        //          'type': 'address'
        //        },
        //        {
        //          'name': '_value',
        //          'type': 'uint256'
        //        }
        //      ],
        //      'name': 'transfer',
        //      'outputs': [
        //        {
        //          'name': '',
        //          'type': 'bool'
        //        }
        //      ],
        //      'payable': false,
        //      'stateMutability': 'nonpayable',
        //      'type': 'function'
        //    }
        //]";

        //    var web3 = new Web3(new Account(privateKey), url);

        //    var contract = web3.Eth.GetContract(abi, usdtContractAddress);
        //    var transferFunction = contract.GetFunction("transfer");

        //    var recipientAddress = "RECIPIENT_ADDRESS";
        //    var amountToSend = Web3.Convert.ToWei(100m); // 100 USDT

        //    try
        //    {
        //        var transactionHash = await transferFunction.SendTransactionAsync(accountAddress, recipientAddress, amountToSend);
        //        Console.WriteLine($"Transaction Hash: {transactionHash}");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error: {ex.Message}");
        //    }
        //}

    }
}
