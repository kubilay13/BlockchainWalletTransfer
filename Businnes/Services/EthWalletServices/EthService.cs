using DataAccessLayer.AppDbContext;
using Entities.Models;
using Entities.Models.EthModels;
using Entities.Models.TronModels;
using Extensions.Data;
using Microsoft.EntityFrameworkCore;
using Nethereum.Contracts;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Signer;
using Nethereum.Util;
using Nethereum.Web3;


namespace ETHWalletApi.Services
{
    public class TransferFunction: FunctionMessage
    {
        public string To { get; set; }
        public BigDecimal Amount { get; set; }
    }

    public class EthService : IEthService
    {
        private readonly Web3 _web3;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly HttpClient _httpClient;
        public EthService(ApplicationDbContext applicationDbContext, HttpClient httpClient)
        {
            _applicationDbContext = applicationDbContext;
            _web3 = new Web3("https://sepolia.infura.io/v3/3fcb68529b9e4288a4eb599f266bbb50");
            _httpClient = httpClient;
        }
        public async Task<WalletModel>CreateETHWalletAsync(string walletName)
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
                var walletDetails = new WalletModel
                {
                    WalletName = walletName,
                    PrivateKeyEth = privateKey,
                    WalletAddressETH = address,
                    ETHAmount = 0,
                    Network = "ETH",
                    WalletTronScanURL = $"https://sepolia.etherscan.io/tx/{address}"
                };
                var ethSaveDbWallet = new WalletModel
                {
                    WalletName = walletName,
                    PrivateKeyTron = "null",
                    PrivateKeyEth = privateKey,
                    WalletAddressTron = "null",
                    WalletAddressETH = address,
                    CreatedAt = DateTime.UtcNow,
                    LastTransactionAt = DateTime.UtcNow,
                    TrxAmount = 0,
                    UsdtAmount = 0,
                    UsdcAmount = 0,
                    ETHAmount = 0,
                    Network = "ETH",
                    WalletEthScanUrl = $"https://sepolia.etherscan.io/tx/{address}",
                    WalletTronScanURL = null,
                    TransactionLimit = false
                };
                _applicationDbContext.WalletModels.Add(ethSaveDbWallet);
                try
                {
                    await _applicationDbContext.SaveChangesAsync();
                }
                catch (DbUpdateException dbEx)
                {
                    Console.WriteLine($"Veritabanı güncelleme hatası: {dbEx.InnerException?.Message ?? dbEx.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Beklenmeyen hata: {ex.Message}");
                    throw;
                }
                return walletDetails;
            }
        }
        public async Task<string> SendTransactionAsyncETH(TransferRequest request)
        {
            var privateKey = await GetPrivateKeyByAddressAsync(request.SenderAddress);
            var account = new Nethereum.Web3.Accounts.Account(privateKey);
            var web3 = new Web3(account, _web3.Client);
            var amountInWei = Web3.Convert.ToWei(request.Amount);
            var currentNonce = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(account.Address);
            try
            {
                var transaction = new TransactionInput
                {
                    From = request.SenderAddress,
                    To = request.ReceiverAddress,
                    Value = new HexBigInteger(amountInWei),
                    Nonce = currentNonce,
                    GasPrice = await web3.Eth.GasPrice.SendRequestAsync()
                };
                var _gas = await web3.Eth.Transactions.EstimateGas.SendRequestAsync(transaction);
                transaction.Gas = _gas;
                var signature = await web3.TransactionManager.SignTransactionAsync(transaction);
                var txnHash = await web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(signature);

                var EthTransaction = new TransferHistoryModel
                {
                    SendingAddress = request.SenderAddress,
                    ReceivedAddress = request.ReceiverAddress,
                    TransactionHash = txnHash,
                    CoinType = "Ethereum",
                    TransactionNetwork = "ETH",
                    TransactionAmount = request.Amount,
                    TransactionDate = DateTime.UtcNow,
                    Commission = 0,
                    NetworkFee = Convert.ToDecimal(_gas.ToString()),
                    TransactionUrl = $"https://sepolia.etherscan.io/tx/{txnHash}",
                    TransactionStatus = true,
                    TransactionType = 0,
                    Network = "TestNet(Sepolia)"
                };
                _applicationDbContext.TransferHistoryModels.Add(EthTransaction);
                await _applicationDbContext.SaveChangesAsync();
                var transactionUrl = $"https://sepolia.etherscan.io/tx/{txnHash}";
                return ("ETH Transfer İşleminiz Başarılı:  "+ transactionUrl);
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

        public async Task<string> SendTransactionAsyncUSDT(EthNetworkTransactionRequest request)
        {
            var bnbcontract = "0x17c3fD32E71b97Ae7EA1B5dCa135846461a8F6B6";
            var usdtcontract = "0x2DCe21ca7F38D7Fbb6Bbf86AC11ec7867A510f24";
            var privateKey = await GetPrivateKeyByAddressAsync(request.FromAddress);
            var account = new Nethereum.Web3.Accounts.Account(privateKey);
            var web3 = new Web3(account, _web3.Client);
            var amountInWei = Web3.Convert.ToWei(request.Amount.Value);
            var currentNonce = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(account.Address);
            try
            {
                var transferFunction = new  TransactionInput
                {
                    From = request.FromAddress,
                    To = request.ToAddress,
                    Value = new HexBigInteger(amountInWei),
                    Nonce = currentNonce,
                    GasPrice = await web3.Eth.GasPrice.SendRequestAsync()
                };
                var transferHandler = web3.Eth.GetContractTransactionHandler<TransferFunction>();
                var transferReceipt = await transferHandler.SendRequestAndWaitForReceiptAsync(usdtcontract/*,functionMessage*/);
                var transaction = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transferReceipt.TransactionHash);
                var transferDecoded = transaction.DecodeTransactionToFunctionMessage<TransferFunction>();

                return transferReceipt.TransactionHash;
            }
            catch (RpcResponseException ex)
            {
                throw new InvalidOperationException($"ETH Transfer İşlemi Başarısız: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("ETH Transfer İşleminde Beklenmeyen Bir Hata Oluştu.", ex);
            }
        }
        public async Task<string> GetPrivateKeyByAddressAsync(string walletAddress)
        {
            var wallet = await _applicationDbContext.WalletModels
                .FirstOrDefaultAsync(w => w.WalletAddressETH == walletAddress);
            if (wallet == null || wallet.PrivateKeyEth == null)
            {
                throw new ApplicationException("Cüzdan bulunamadı veya privateKey mevcut değil.");
            }
            return wallet.PrivateKeyEth;
        }

        
    }
}
