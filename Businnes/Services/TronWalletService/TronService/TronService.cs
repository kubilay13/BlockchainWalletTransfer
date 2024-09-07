using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;
using TronNet;
using HDWallet.Core;
using TronNet.Crypto;
using Newtonsoft.Json;
using Google.Protobuf;
using TronNet.Contracts;
using Transaction = TronNet.Protocol.Transaction;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Entities.Models.TronModels;
using Entities.Enums;
using DataAccessLayer.AppDbContext;
using Entities.Models.WalletModel;
using Entities.Dto.TronDto;
using Entities.Dto.WalletApiDto;
using Business.Services.WalletPrivatekeyToPasswords;
using Entities.Models.AdminModel;

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
    private readonly IWalletPrivatekeyToPassword _walletPrivatekeyToPassword;
    public TronService(HttpClient client, ITronClient tronClient, ApplicationDbContext applicationDbContext, IWalletClient walletClient, ITransactionClient transactionClient, HttpClient httpClient, ILogger<TronService> logger, IContractClientFactory contractClientFactory, IConfiguration configuration, IWalletPrivatekeyToPassword walletPrivatekeyToPassword)
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
        _httpClient.BaseAddress = new Uri("https://api.nile.trongrid.io/");
        _httpClient = httpClient;
        _logger = logger;
        _contractClientFactory = contractClientFactory;
        _configuration = configuration;
        _walletPrivatekeyToPassword = walletPrivatekeyToPassword;
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
            var wallet = await _applicationDbContext.WalletDetailModels.FirstOrDefaultAsync(w => w.WalletAddressTron == receiverAddress);
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
    public async Task<decimal> GetBalanceAsyncTrxBackgroundService(string address)
    {
        string url = $"https://api.trongrid.io/v1/accounts/{address}";
        var response = await _client.GetStringAsync(url);
        var json = JObject.Parse(response);
        decimal balance = json["data"][0]["balance"].Value<decimal>() / 1000000m;
        return balance;
    }
    public async Task<List<AssetBalance>> GetAllWalletBalanceAsyncTron(string address, byte[] encryptedPrivateKey)
    {
        try
        {
            var decryptedPrivateKey = _walletPrivatekeyToPassword.DecryptPrivateKey(encryptedPrivateKey);
            var hexAddress = Base58Encoder.DecodeFromBase58Check(address).ToHexString();
            string apiUrl = $"/v1/accounts/{hexAddress}";
            var response = await _client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonObject = JObject.Parse(responseBody);
            var assetsList = new List<AssetBalance>();

            if (jsonObject["data"] != null && jsonObject["data"].HasValues)
            {
                var data = jsonObject["data"][0];
                if (data["balance"] != null)
                {
                    decimal trxBalance = decimal.Parse(data["balance"].ToString()) / 1000000m;
                    assetsList.Add(new AssetBalance { AssetName = "TRX", Balance = trxBalance });
                }

                // TRC10 tokenlerini ekle
                if (data["assetV2"] != null)
                {
                    foreach (var asset in data["assetV2"])
                    {
                        string assetName = asset["key"].ToString();
                        decimal assetBalance = decimal.Parse(asset["value"].ToString()) / 1000000m;
                        assetsList.Add(new AssetBalance { AssetName = assetName, Balance = assetBalance });
                    }
                }

                // TRC20 tokenlerini ekle
                if (data["trc20"] != null)
                {
                    foreach (var trc20Token in data["trc20"])
                    {
                        foreach (var property in trc20Token.Children<JProperty>())
                        {
                            string assetName = property.Name;
                            decimal assetBalance = decimal.Parse(property.Value.ToString()) / 1000000m;
                            assetsList.Add(new AssetBalance { AssetName = assetName, Balance = assetBalance });
                        }
                    }
                }
            }
            else
            {
                throw new ApplicationException("Cüzdana ait varlık bilgileri bulunamadı.");
            }

            return assetsList;
        }
        catch (HttpRequestException ex)
        {
            throw new ApplicationException("API ile iletişim sırasında bir hata oluştu.", ex);
        }
    }
    public async Task WalletSaveHistoryToken(TransferRequest request)
    {
        var senderAddress = await _applicationDbContext.WalletDetailModels.FirstOrDefaultAsync(w => w.WalletAddressTron == request.SenderAddress);
        var network = await _applicationDbContext.Networks.FirstOrDefaultAsync(n => n.Type == NetworkType.Network && n.Name == request.CoinName);
        byte[] encryptedPrivateKeyBytes = Convert.FromBase64String(senderAddress.PrivateKeyTron);
        string decryptedPrivateKey = _walletPrivatekeyToPassword.DecryptPrivateKey(encryptedPrivateKeyBytes);
        var senderprivatekey = await GetPrivateKeyFromDatabase(request.SenderAddress);
        var account = _walletClient.GetAccount(Convert.ToString(decryptedPrivateKey));
        decimal commissionPercentage = network.Commission;
        decimal commission = request.Amount - commissionPercentage;
        var FeeAmount = 0 * 1000000L;
        var contractClient = _contractClientFactory.CreateClient(ContractProtocol.TRC20);
        var transferResult = await contractClient.TransferAsync(network.Contract, account, request.ReceiverAddress, Convert.ToDecimal(request.Amount), string.Empty, FeeAmount);
        if (transferResult != null)
        {
            var historyModel = new TransferHistoryModel
            {
                SendingAddress = request.SenderAddress,
                ReceivedAddress = request.ReceiverAddress,
                CoinType = request.CoinName,
                TransactionNetwork = "TRC20",
                TransactionAmount = request.Amount,
                TransactionDate = DateTime.UtcNow,
                Commission = commissionPercentage,
                NetworkFee = 0,
                TransactionUrl = $"https://nile.tronscan.org/#/transaction/{transferResult}",
                TransactionStatus = true,
                TransactionType = request.TransactionType,
                TransactionHash = transferResult,
                Network = request.Network
            };
            _applicationDbContext.TransferHistoryModels.Add(historyModel);
            await _applicationDbContext.SaveChangesAsync();
        }
        else
        {
            var historyModel = new TransferHistoryModel
            {
                SendingAddress = request.SenderAddress,
                ReceivedAddress = request.ReceiverAddress,
                CoinType = request.CoinName,
                TransactionNetwork = "TRC20",
                TransactionAmount = request.Amount,
                TransactionDate = DateTime.UtcNow,
                Commission = commissionPercentage,
                NetworkFee = 0,
                TransactionUrl = $"https://nile.tronscan.org/#/transaction/{transferResult}",
                TransactionStatus = false,
                TransactionType = request.TransactionType,
                TransactionHash = transferResult,
                Network = request.Network
            };
            _applicationDbContext.TransferHistoryModels.Add(historyModel);
            await _applicationDbContext.SaveChangesAsync();
        }
        await Task.CompletedTask;
    }
    public async Task<decimal> GetTronUsdApiPriceAsync()
    {
        string url = "https://api.coingecko.com/api/v3/simple/price?ids=tron&vs_currencies=usd";
        var response = await _client.GetStringAsync(url);
        var json = JObject.Parse(response);
        decimal tronPriceInUsd = json["tron"]["usd"].Value<decimal>();
        return tronPriceInUsd;
    }
    public async Task WalletTokenAdminComission(TransferRequest request)
    {
        var network = await _applicationDbContext.Networks.FirstOrDefaultAsync(n => n.Type == NetworkType.Network && n.Name == request.CoinName);
        if (network == null)
        {
            throw new InvalidOperationException("Network bilgileri bulunamadı.");
        }
        var transactionCommission = (request.Amount * network.Commission) / 100;
        var senderPrivateKey = await GetPrivateKeyFromDatabase(request.SenderAddress);
        var AdmintransactionClient = _tronClient.GetTransaction();
        var AdminsignedTransaction = await _transactionClient.CreateTransactionAsync(request.SenderAddress, network.AdminWallet, (long)transactionCommission * 1000000);
        var senderAddress = await _applicationDbContext.WalletDetailModels.FirstOrDefaultAsync(w => w.WalletAddressTron == request.SenderAddress);
        if (senderAddress == null)
        {
            throw new ApplicationException("Gönderici cüzdan adresi bulunamadı.");
        }
        byte[] encryptedPrivateKeyBytes = Convert.FromBase64String(senderAddress.PrivateKeyTron);
        string decryptedPrivateKey = _walletPrivatekeyToPassword.DecryptPrivateKey(encryptedPrivateKeyBytes);
        var account = _walletClient.GetAccount(decryptedPrivateKey);
        var AdmintransactionSigned = _transactionClient.GetTransactionSign(AdminsignedTransaction.Transaction, decryptedPrivateKey);
        var Adminresult = await _transactionClient.BroadcastTransactionAsync(AdmintransactionSigned);
        await Task.CompletedTask;
    }
    private async Task WalletTransferQuery(TransferRequest request, WalletDetailModel wallet)
    {
        if (request.SenderAddress == request.ReceiverAddress)
        {
            throw new ApplicationException("Alıcı Cüzdan Adresiyle Gönderici Adres Aynılar.");
        }
        if (wallet.UsdtAmount < request.Amount && wallet.UsdtAmount < request.Amount && wallet.UsdcAmount < request.Amount)
        {
            throw new ApplicationException("Transfer İşlemini Karşılayacak Miktar Cüzdanınızda Yok.");
        }
        if (wallet.UsdcAmount == 0 && wallet.UsdcAmount == 0 && wallet.TrxAmount == 0)
        {
            throw new ApplicationException("Cüzdan Bakiyeniz 0");
        }
        if (request.SenderAddress == null && request.ReceiverAddress == null)
        {
            throw new ApplicationException("Alıcı Veya Gönderici Adres Boş.");
        }
        if (request.CoinName == null)
        {
            throw new ApplicationException("USDT,USDC,TRX Sadece Bunları Yazıp Transfer Edebilirsiniz.");
        }
        await Task.CompletedTask;
    }
    public async Task<string> GetPrivateKeyFromDatabase(string senderadress)
    {
        var wallet = await _applicationDbContext.WalletDetailModels.FirstOrDefaultAsync(q => q.WalletAddressTron == senderadress);
        return wallet?.PrivateKeyTron!;
    }
    private async Task UpdateValue(TransferRequest request, long scaledAmount, string senderPrivateKey)
    {
        var transactionClient = _tronClient.GetTransaction();
        var transactionExtension = await transactionClient.CreateTransactionAsync(request.SenderAddress, request.ReceiverAddress, scaledAmount);
        var signedTransaction = transactionClient.GetTransactionSign(transactionExtension.Transaction, senderPrivateKey);
        var result = await transactionClient.BroadcastTransactionAsync(signedTransaction);
    }
    //private async Task<bool> TransferLimitControl(TransferRequest request)
    //{
    //    //var Commission = await _applicationDbContext.Networks.FirstOrDefaultAsync(w => w.Name == request.CoinName);
    //    //var senderWallet = await _applicationDbContext.WalletDetailModels.FirstOrDefaultAsync(w => w.WalletAddressTron == request.SenderAddress);
    //    //var _comission = Commission!.Commission;
    //    //if (request.TransactionType != TransactionType.Deposit)
    //    //{
    //    //    var twentyFourHoursAgo = DateTime.UtcNow.AddHours(-24);
    //    //    var dailyTransfers = await _applicationDbContext.TransferHistoryModels
    //    //        .Where(t => t.SendingAddress == request.SenderAddress && t.TransactionDate >= twentyFourHoursAgo && t.TransactionStatus == true)
    //    //        .SumAsync(t => t.TransactionAmount);
    //    //    if (dailyTransfers + request.Amount > 1000)
    //    //    {
    //    //        throw new ApplicationException("Günlük transfer sınırını aştınız.");
    //    //    }
    //    //}
    //    //if (senderWallet == null)
    //    //{
    //    //    throw new ApplicationException($"Gönderen adres {request.SenderAddress} bulunamadı.");
    //    //}
    //    //if (request.Amount <= 0)
    //    //{
    //    //    throw new ApplicationException("Gönderilecek miktar 0'dan büyük olmalıdır.");
    //    //}
    //    //if (senderWallet.TrxAmount < request.Amount /*+_comission)
    //    //{
    //    //    throw new ApplicationException($"Yetersiz bakiye.Bakiye {request.Amount + _comission} tutarından fazla olmalıdır.");
    //    //}
    //    //var receiverWallet = await _applicationDbContext.WalletDetailModels.FirstOrDefaultAsync(w => w.WalletAddressTron == request.ReceiverAddress);
    //    //return true;
    //}
    public string GetTransactionHash(Transaction signedTransaction)
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
    public async Task<string> AdminLogin(AdminLoginModel adminLoginModel)
    {
        var admin = await _applicationDbContext.AdminLoginModels.SingleOrDefaultAsync(a => a.Username == adminLoginModel.Username);
        if (admin == null)
        {
            return ("Kullanıcı adı bulunamadı.");
        }
        if (admin.Password != adminLoginModel.Password)
        {
            return ("Yanlış şifre.");
        }
        return ("Giriş başarılı.");
    }
    public async Task<string> UserLogin(UserLoginRequestDto userLoginRequestDto)
    {
        var userlogin = await _applicationDbContext.userLoginModels.SingleOrDefaultAsync(a => a.UserMailAdress == userLoginRequestDto.Email);
        if (userlogin == null)
        {
            return ("Kayıtlı Mail Bulunamadı.");
        }
        else
        {
            if (userlogin.Password != userLoginRequestDto.Password)
            {
                return ("yanlış şifre girdiniz.");
            }
        }
        return ("giriş başarılı hogeldiniz.");
    }
}











