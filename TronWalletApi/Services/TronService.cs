using Newtonsoft.Json.Linq;
using TronWalletApi.Context;
using TronWalletApi.Models;
using Microsoft.EntityFrameworkCore;
using TronNet;
using HDWallet.Core;
using TronNet.Crypto;
using Newtonsoft.Json;
using Google.Protobuf;
using TronWalletApi.Models.TransactionModel;
using TronNet.Contracts;
using Transaction = TronNet.Protocol.Transaction;
using Nethereum.Util;
using TronWalletApi.Enums;
using Serilog;
using static NBitcoin.Scripting.OutputDescriptor;
public class TronService : ITronService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _client;
    private readonly ITronClient _tronClient;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IWalletClient _walletClient;
    private readonly ITransactionClient _transactionClient;
    private readonly HttpClient _httpClient;
    private readonly ILogger<TronService> _logger;
    private readonly IContractClientFactory _contractClientFactory;
    public TronService(HttpClient client, ITronClient tronClient, ApplicationDbContext applicationDbContext, IWalletClient walletClient, ITransactionClient transactionClient, HttpClient httpClient, ILogger<TronService> logger, IContractClientFactory contractClientFactory, IConfiguration configuration)
    {
        _client = client;
        _client.BaseAddress = new Uri("https://nile.trongrid.io");
        _client.DefaultRequestHeaders.Add("TRON-PRO-API-KEY", "bbf6d1c9-daf4-49d9-a088-df29f664bac9");
        _tronClient = tronClient;
        _applicationDbContext = applicationDbContext;
        _walletClient = walletClient;
        _client.BaseAddress = new Uri("https://nile.trongrid.io");
        _transactionClient = transactionClient;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://api.shasta.trongrid.io/");
        _httpClient = httpClient;
        _logger = logger;
        _contractClientFactory = contractClientFactory;
        _configuration = configuration;
    }
    public async Task<string> CreateWallet(string walletName)
    {
        try
        {
            var ecKey = TronECKey.GenerateKey(TronNetwork.MainNet);
            var privateKey = ecKey.GetPrivateKey();
            var address = ecKey.GetPublicAddress();
            var wallet = new TronWalletModel
            {
                WalletName = walletName,
                PrivateKey = privateKey,
                WalletAddress = address,
                CreatedAt = DateTime.UtcNow,
                CreatedAtTime = DateTime.Now.ToString("HH:mm:ss"),
                WalletTronScanURL = $"https://nile.tronscan.org/#/address/{address}",
                Network = "Testnet(Nile)"
            };
            _applicationDbContext.TronWalletModels.Add(wallet);
            await _applicationDbContext.SaveChangesAsync();
            var network = await _applicationDbContext.Networks.FirstOrDefaultAsync(n => n.Type == NetworkType.Network);
            string adminAddress = network.AdminWallet;
            await SendTronAsync(adminAddress, address, 20000000);
            var response = $"PrivateKey: {wallet.PrivateKey}\nWalletAdress: {wallet.WalletAddress}";
            return response;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Tron cüzdanı oluşturma işlemi başarısız oldu.", ex);
        }
    }
    public async Task SendTronAsync(string senderAddress, string receiverAddress, long amount)
    {
        try
        {
            Console.WriteLine($"SendTronAsync içinde amount: {amount}");
            var transactionClient = _tronClient.GetTransaction();
            var signedTransaction = await _transactionClient.CreateTransactionAsync(senderAddress, receiverAddress, amount);
            var network = await _applicationDbContext.Networks.FirstOrDefaultAsync(n => n.Type == NetworkType.Network);
            string adminprivatekey = network.AdminWalletPrivateKey;
            var transactionSigned = _transactionClient.GetTransactionSign(signedTransaction.Transaction, adminprivatekey);
            var result = await _transactionClient.BroadcastTransactionAsync(transactionSigned);
            if (!result.Result)
            {
                throw new ApplicationException("TRX gönderimi başarısız oldu.");
            }
            var wallet = await _applicationDbContext.TronWalletModels.FirstOrDefaultAsync(w => w.WalletAddress == receiverAddress);
            if (wallet != null)
            {
                wallet.TrxAmount += amount / 1000000;
                await _applicationDbContext.SaveChangesAsync();
            }
            else
            {
                throw new ApplicationException("Amount değerini dbye kaydetme işleminde sorunla karşılaşıldı.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
            throw;
        }
    }
    public async Task<decimal> GetBalanceAsync(string address)
    {
        try
        {
            var hexAddress = Base58Encoder.DecodeFromBase58Check(address).ToHexString();
            string apiUrl = $"/wallet/getaccount?address={hexAddress}";
            var response = await _client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Response Body:");
            Console.WriteLine(responseBody);
            var jsonObject = JObject.Parse(responseBody);
            if (jsonObject["balance"] != null && decimal.TryParse(jsonObject["balance"].ToString(), out decimal balance))
            {
                return balance / 1000000m;
            }
            else
            {
                throw new ApplicationException("API'den beklenen 'balance' özelliği bulunamadı veya geçerli bir değer değil.");
            }
        }
        catch (HttpRequestException ex)
        {
            throw new ApplicationException("API ile iletişim sırasında bir hata oluştu.", ex);
        }
    }
    public async Task<decimal> GetBalanceAsyncUsdt(string UsdtBalance, string privatekey)
    {
        try
        {
            var account = _walletClient.GetAccount(privatekey);
            var protocol = _contractClientFactory.CreateClient(ContractProtocol.TRC20);
            var usdtbalance = await protocol.BalanceOfAsync(_configuration.GetValue<string>("Contract:Usdt"), account);
            if (usdtbalance != null)
            {
                return usdtbalance;
            }
            else
            {
                throw new ApplicationException("API'den beklenen 'balance' özelliği bulunamadı veya geçerli bir değer değil.");
            }
        }
        catch (HttpRequestException ex)
        {
            throw new ApplicationException("API ile iletişim sırasında bir hata oluştu.", ex);
        }
    }
    public async Task<decimal> GetBalance(string address)
    {
        string url = $"https://api.trongrid.io/v1/accounts/{address}";
        var response = await _client.GetStringAsync(url);
        var json = JObject.Parse(response);
        decimal balance = json["data"][0]["balance"].Value<decimal>() / 1000000m;
        return balance;
    }
    public async Task<decimal> GetTronUsdApiPriceAsync()
    {
        string url = "https://api.coingecko.com/api/v3/simple/price?ids=tron&vs_currencies=usd";
        var response = await _client.GetStringAsync(url);
        var json = JObject.Parse(response);
        decimal tronPriceInUsd = json["tron"]["usd"].Value<decimal>();
        return tronPriceInUsd;
    }
    public async Task Transfer(TransferRequest request)
    {
        switch (request!.CoinName!.ToUpper())
        {
            case "TRX":
                await TrxTransfer(request);
                break;

            default:
                await TokenTransfer(request);
                break;  
        }
    }
    private async Task TrxTransfer(TransferRequest request)
    {
        try
        {
            if (request != null)
            {
                if (request.SenderAddress == request.ReceiverAddress)
                {
                    throw new ApplicationException("Alıcı Cüzdan Adresiyle Gönderici Adres Aynılar.");
                }
                var _transferLimit= await TransferControl(request);

                if (!_transferLimit)
                {
                    throw new ApplicationException("Transfer işlemi başarısız oldu.");
                }
                var _network = await _applicationDbContext.Networks
                    .FirstOrDefaultAsync(w => w.Name == request.CoinName);
                var network = await _applicationDbContext.Networks.FirstOrDefaultAsync(n => n.Type == NetworkType.Network && n.Name == request.CoinName);
                string adminAddress = network.AdminWallet;
                var _comission =_network!.Commission;
                var _amount = UnitConversion.Convert.ToWei(request.Amount, (int)_network.Decimal);
                var senderAddress = await _applicationDbContext.TronWalletModels.FirstOrDefaultAsync(w => w.WalletAddress == request.SenderAddress);
                var trxamount = await _applicationDbContext.TronWalletModels.FirstOrDefaultAsync(n => n.TrxAmount == request.Amount);
              
                if (_amount > 0)
                {
                    var transactionClient = _tronClient.GetTransaction();
                    var signedTransaction = await _transactionClient.CreateTransactionAsync(request.SenderAddress, request.ReceiverAddress, (long)_amount);
                    var transactionSigned = _transactionClient.GetTransactionSign(signedTransaction.Transaction, senderAddress!.PrivateKey);
                    var result = await _transactionClient.BroadcastTransactionAsync(transactionSigned);
                    if (!result.Result)
                    {
                        throw new ApplicationException($"Trx transfer işlemi başarısız oldu. Hata mesajı: {result.Message}");
                    }
                    var transactionHash = GetTransactionHash(transactionSigned);
                    if (transactionHash != null)
                    {
                        var transferHistory = new TransferHistoryModel
                        {
                            SendingAddress = request.SenderAddress,
                            ReceivedAddress = request.ReceiverAddress,
                            CoinType = request.CoinName!.ToUpper(),
                            TransactionNetwork = "TRON",
                            TransactionAmount = request.Amount,
                            TransactionDate = DateTime.UtcNow,
                            TransactionDateTime = DateTime.Now.ToString("HH:mm:ss"),
                            Commission = _comission,
                            NetworkFee = 0,
                            SenderTransactionUrl = $"https://nile.tronscan.org/#/address/{request.SenderAddress}",
                            ReceiverTransactionUrl = $"https://nile.tronscan.org/#/address/{request.ReceiverAddress}",
                            TransactionUrl = $"https://nile.tronscan.org/#/transaction/{transactionHash}",
                            TransactionStatus = true,
                            TransactionType = request.TransactionType,
                            TransactionHash = transactionHash,
                        };
                        Log.Error("TRX Transfer İşlemi Başarılı.");
                        _applicationDbContext.TransferHistoryModels.Add(transferHistory);
                        await _applicationDbContext.SaveChangesAsync();
                    }
                    else
                    {
                        var transferHistory = new TransferHistoryModel
                        {
                            SendingAddress = request.SenderAddress,
                            ReceivedAddress = request.ReceiverAddress,
                            CoinType = request.CoinName!.ToUpper(),
                            TransactionNetwork = "TRON",
                            TransactionAmount = request.Amount,
                            TransactionDate = DateTime.UtcNow,
                            TransactionDateTime = DateTime.Now.ToString("HH:mm:ss"),
                            Commission = _comission,
                            NetworkFee = 0,
                            SenderTransactionUrl = $"https://nile.tronscan.org/#/address/{request.SenderAddress}",
                            ReceiverTransactionUrl = $"https://nile.tronscan.org/#/address/{request.ReceiverAddress}",
                            TransactionUrl = $"https://nile.tronscan.org/#/transaction/{transactionHash}",
                            TransactionStatus = false,
                            TransactionType = request.TransactionType,
                            TransactionHash = null,
                        };
                        Log.Error("TRX Transfer İşlemi Sırasında Bir Sorun Oluştu.");
                        _applicationDbContext.TransferHistoryModels.Add(transferHistory);
                        await _applicationDbContext.SaveChangesAsync();
                    }
                    if (transactionHash != null)
                    {
                        senderAddress.TrxAmount -= request.Amount;
                        _applicationDbContext.TronWalletModels.Update(senderAddress);
                        var receiverAddress = await _applicationDbContext.TronWalletModels.FirstOrDefaultAsync(w => w.WalletAddress == request.ReceiverAddress);
                        if (receiverAddress != null)
                        {
                            receiverAddress.TrxAmount += request.Amount;
                            _applicationDbContext.TronWalletModels.Update(receiverAddress);
                        }
                        await _applicationDbContext.SaveChangesAsync();
                    }
                    var transactionCommission = (request.Amount * network.Commission) / 100 ;
                    var AdmintransactionClient = _tronClient.GetTransaction();
                    var AdminsignedTransaction = await _transactionClient.CreateTransactionAsync(request.SenderAddress, network.AdminWallet, (long)transactionCommission * 1000000);
                    var AdmintransactionSigned = _transactionClient.GetTransactionSign(AdminsignedTransaction.Transaction, senderAddress!.PrivateKey);
                    var Adminresult = await _transactionClient.BroadcastTransactionAsync(AdmintransactionSigned);
                }
            }
        }
        catch (ApplicationException ex)
        {
            _logger.LogError($"Transfer işlemi başarısız oldu. Hata mesajı: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Transfer işlemi başarısız oldu. Hata mesajı: {ex.Message}");
            throw new ApplicationException("Bilinmeyen bir hata oluştu.");
        }
    }
    public async Task TokenTransfer(TransferRequest request)
    {
        var network = await _applicationDbContext.Networks.FirstOrDefaultAsync(n => n.Type == NetworkType.Network && n.Name == request.CoinName);
        var senderprivatekey = await GetPrivateKeyFromDatabase(request.SenderAddress);
        var account = _walletClient.GetAccount(senderprivatekey);
        var feeAmount = 5 * 1000000L;
        decimal commissionPercentage = network.Commission;
        decimal commission = request.Amount - commissionPercentage;
        var contractClient = _contractClientFactory.CreateClient(ContractProtocol.TRC20);
        var wallet = await _applicationDbContext.TronWalletModels.FirstOrDefaultAsync(q => q.WalletAddress == request.SenderAddress);
        string senderadress = wallet.PrivateKey;
        if (request.CoinName == "USDT")
        {

            if (request.SenderAddress == request.ReceiverAddress)
            {
                throw new ApplicationException("Alıcı Cüzdan Adresiyle Gönderici Adres Aynılar.");
            }
            if (wallet.UsdtAmount < request.Amount && wallet.UsdtAmount == 0)
            {
                throw new ApplicationException("Transfer İşlemini Karşılayacak Miktar Cüzdanınızda Yok.");
            }
            else
            {
                if (wallet.TrxAmount >= commission)
                {
                    var transferResult = await contractClient.TransferAsync(
                    network.Contract,
                    account,
                    request.ReceiverAddress,
                    request.Amount,
                    string.Empty,
                    feeAmount
                    );
                    if (transferResult == null)
                    {
                        var successError = new TransactionErrorHistoryModel
                        {
                            SendingAddress = request.SenderAddress,
                            ReceivedAddress = request.ReceiverAddress,
                            CoinType = request.CoinName,
                            TransactionNetwork = "TRC20",
                            TransactionAmount = request.Amount,
                            TransactionDate = DateTime.UtcNow,
                            TransactionDateTime = DateTime.Now.ToString("HH:mm:ss"),
                            TransferFee = commissionPercentage,
                            NetworkFee = 0,
                            SenderTransactionUrl = $"https://nile.tronscan.org/#/address/{request.SenderAddress}",
                            ReceiverTransactionUrl = $"https://nile.tronscan.org/#/address/{request.ReceiverAddress}",
                            TransactionUrl = $"https://nile.tronscan.org/#/transaction/{null}",
                            TransactionStatus = false,
                            TransactionType = request.TransactionType,
                            TransactionHash = null,
                        };
                        _applicationDbContext.TransactionErrorHistoryModels.Add(successError);
                        var historyModel = new TransferHistoryModel
                        {
                            SendingAddress = request.SenderAddress,
                            ReceivedAddress = request.ReceiverAddress,
                            CoinType = request.CoinName,
                            TransactionNetwork = "TRC20",
                            TransactionAmount = request.Amount,
                            TransactionDate = DateTime.UtcNow,
                            TransactionDateTime = DateTime.Now.ToString("HH:mm:ss"),
                            Commission = commissionPercentage,
                            NetworkFee = 0,
                            SenderTransactionUrl = $"https://nile.tronscan.org/#/address/{request.SenderAddress}",
                            ReceiverTransactionUrl = $"https://nile.tronscan.org/#/address/{request.ReceiverAddress}",
                            TransactionUrl = $"https://nile.tronscan.org/#/transaction/{null}",
                            TransactionStatus = false,
                            TransactionType = request.TransactionType,
                            TransactionHash = null,
                        };
                        _applicationDbContext.TransferHistoryModels.Add(historyModel);
                        await _applicationDbContext.SaveChangesAsync();
                    }
                    else
                    {
                        var transactionCommission = (request.Amount * network.Commission) / 100;
                        var AdmintransactionClient = _tronClient.GetTransaction();
                        var AdminsignedTransaction = await _transactionClient.CreateTransactionAsync(request.SenderAddress, network.AdminWallet, (long)transactionCommission * 1000000);
                        var AdmintransactionSigned = _transactionClient.GetTransactionSign(AdminsignedTransaction.Transaction, account.PrivateKey);
                        var Adminresult = await _transactionClient.BroadcastTransactionAsync(AdmintransactionSigned);
                        var historyModel = new TransferHistoryModel
                        {
                            SendingAddress = request.SenderAddress,
                            ReceivedAddress = request.ReceiverAddress,
                            CoinType = request.CoinName,
                            TransactionNetwork = "TRC20",
                            TransactionAmount = request.Amount,
                            TransactionDate = DateTime.UtcNow,
                            TransactionDateTime = DateTime.Now.ToString("HH:mm:ss"),
                            Commission = commissionPercentage,
                            NetworkFee = 0,
                            SenderTransactionUrl = $"https://nile.tronscan.org/#/address/{request.SenderAddress}",
                            ReceiverTransactionUrl = $"https://nile.tronscan.org/#/address/{request.ReceiverAddress}",
                            TransactionUrl = $"https://nile.tronscan.org/#/transaction/{transferResult}",
                            TransactionStatus = true,
                            TransactionType = request.TransactionType,
                            TransactionHash = transferResult,
                        };
                        _applicationDbContext.TransferHistoryModels.Add(historyModel);
                        await _applicationDbContext.SaveChangesAsync();
                    }
                    if (transferResult != null)
                    {
                        var senderAddress = await _applicationDbContext.TronWalletModels.FirstOrDefaultAsync(w => w.WalletAddress == request.SenderAddress);
                        senderAddress.UsdtAmount -= request.Amount;
                        _applicationDbContext.TronWalletModels.Update(senderAddress);
                        var receiverAddress = await _applicationDbContext.TronWalletModels.FirstOrDefaultAsync(w => w.WalletAddress == request.ReceiverAddress);
                        if (receiverAddress != null)
                        {
                            receiverAddress.TrxAmount += request.Amount;
                            _applicationDbContext.TronWalletModels.Update(receiverAddress);
                        }
                        await _applicationDbContext.SaveChangesAsync();
                    }
                }
            }
        }
        else
        {
            var UsdcAmount = 40 * 1000000L;
            if (wallet.TrxAmount >= commission)
            {
                var transferResult = await contractClient.TransferAsync(
                _configuration.GetValue<string>("Contract:Usdc"),
                account,
                request.ReceiverAddress,
                request.Amount,
                string.Empty,
                UsdcAmount
                );
                var transactionCommission = (request.Amount * network.Commission) / 100;
                var AdmintransactionClient = _tronClient.GetTransaction();
                var AdminsignedTransaction = await _transactionClient.CreateTransactionAsync(request.SenderAddress, network.AdminWallet, (long)transactionCommission * 1000000);
                var AdmintransactionSigned = _transactionClient.GetTransactionSign(AdminsignedTransaction.Transaction, account.PrivateKey);
                var Adminresult = await _transactionClient.BroadcastTransactionAsync(AdmintransactionSigned);
            }
        }
    }
    private async Task<string> GetPrivateKeyFromDatabase(string senderadress)
    {
        var wallet = await _applicationDbContext.TronWalletModels.FirstOrDefaultAsync(q => q.WalletAddress == senderadress);
        return wallet?.PrivateKey!;
    }
    private async Task UpdateValue(TransferRequest request, long scaledAmount, string senderPrivateKey)
    {
        var transactionClient = _tronClient.GetTransaction();
        var transactionExtension = await transactionClient.CreateTransactionAsync(request.SenderAddress, request.ReceiverAddress, scaledAmount);
        var signedTransaction = transactionClient.GetTransactionSign(transactionExtension.Transaction, senderPrivateKey);
        var result = await transactionClient.BroadcastTransactionAsync(signedTransaction);
    }
    private async Task<bool> TransferControl(TransferRequest request)
    {
        var Commission = await _applicationDbContext.Networks
        .FirstOrDefaultAsync(w => w.Name == request.CoinName);
        var _comission = Commission!.Commission;
        var senderWallet = await _applicationDbContext.TronWalletModels
           .FirstOrDefaultAsync(w => w.WalletAddress == request.SenderAddress);
        if (request.TransactionType != TronWalletApi.Enums.TransactionType.Deposit)
        {
            var twentyFourHoursAgo = DateTime.UtcNow.AddHours(-24);
            var dailyTransfers = await _applicationDbContext.TransferHistoryModels
                .Where(t => t.SendingAddress == request.SenderAddress && t.TransactionDate >= twentyFourHoursAgo && t.TransactionStatus == true)
                .SumAsync(t => t.TransactionAmount);
            if (dailyTransfers + request.Amount > 1000)
            {
                throw new ApplicationException("Günlük transfer sınırını aştınız.");
            }
        }
        if (senderWallet == null)
        {
            throw new ApplicationException($"Gönderen adres {request.SenderAddress} bulunamadı.");
        }
        if (request.Amount <= 0)
        {
            throw new ApplicationException("Gönderilecek miktar 0'dan büyük olmalıdır.");
        }
        if (senderWallet.TrxAmount < request.Amount /*+_comission*/)
        {
            throw new ApplicationException($"Yetersiz bakiye.Bakiye {request.Amount + _comission} tutarından fazla olmalıdır.");
        }
        var receiverWallet = await _applicationDbContext.TronWalletModels
             .FirstOrDefaultAsync(w => w.WalletAddress == request.ReceiverAddress);
        return true;
    }
    private string GetTransactionHash(Transaction signedTransaction)
    {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            var hash = sha256.ComputeHash(signedTransaction.RawData.ToByteArray());
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
    public async Task<TransactionInfoModel> GetTransactionFeeAsync(string transactionHash)
    {
        try
        {
            var response = await _httpClient.GetAsync($"https://nile.trongrid.io/walletsolidity/gettransactioninfobyid?value={transactionHash}");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine("API Yanıtı: " + responseString);
            if (string.IsNullOrWhiteSpace(responseString))
            {
                throw new ApplicationException($"Hash değeri '{transactionHash}' için API yanıtı boş.");
            }
            var transactionInfo = JsonConvert.DeserializeObject<TransactionInfoModel>(responseString);
            if (transactionInfo == null)
            {
                throw new ApplicationException($"Hash değeri '{transactionHash}' için işlem bilgisi bulunamadı.");
            }
            if (transactionInfo.Receipt == null)
            {
                throw new ApplicationException($"Hash değeri '{transactionHash}' için işlem ücreti bilgisi eksik. Yanıt: {responseString}");
            }
            var netUsage = transactionInfo.Receipt.NetUsage;
            var fee = transactionInfo.Fee;
            return new TransactionInfoModel
            {
                Fee = fee,
                Receipt = new Receipt
                {
                    NetUsage = netUsage
                }
            };
        }
        catch (HttpRequestException ex)
        {
            throw new ApplicationException($"Tron API'sine erişimde hata oluştu: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"İşlem ücretini alma sırasında bir hata oluştu: {ex.Message}", ex);
        }
    }
}











