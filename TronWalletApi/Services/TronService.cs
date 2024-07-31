using Newtonsoft.Json.Linq;
using TronWalletApi.Context;
using TronWalletApi.Models;
using Microsoft.EntityFrameworkCore;
using TronNet;
using HDWallet.Core;
using TronNet.Crypto;
using TronNet.Protocol;
using TronWalletApi.Enums;
using Newtonsoft.Json;
using System.Security.Cryptography;
using Google.Protobuf;
using TronWalletApi.BackgroundServices;
using TronWalletApi.Models.TransactionModel;
using TronNet.Contracts;
using NBitcoin.Secp256k1;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.Signer;
using static TronNet.Protocol.Transaction.Types;
using Microsoft.AspNetCore.Mvc;
using Transaction = TronNet.Protocol.Transaction;
using Nethereum.Hex.HexTypes;
using Nethereum.Util;
public class TronService : ITronService
{
    private readonly HttpClient _client;
    private readonly ITronClient _tronClient;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IWalletClient _walletClient;
    private readonly ITransactionClient _transactionClient;
    private readonly HttpClient _httpClient;
    private readonly ILogger<TronWalletAmountUpdateService> _logger;
    private readonly IContractClientFactory _contractClientFactory;
    private readonly string _usdtContractAddress = "TXYZopYRdj2D9XRtbG411XZZ3kM5VkAeBf";

    public TronService(HttpClient client, ITronClient tronClient, ApplicationDbContext applicationDbContext, IWalletClient walletClient, ITransactionClient transactionClient, HttpClient httpClient, ILogger<TronWalletAmountUpdateService> logger, IContractClientFactory contractClientFactory)
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

            var senderAddress = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N";

            await SendTronAsync(senderAddress, address, 20000000);
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

            var transactionSigned = _transactionClient.GetTransactionSign(signedTransaction.Transaction, "0107932b30922231adff71b4b7c0b05bc948632f56c2b62f98bd18fefeae8a9e");

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
    /// <summary>
    /// Bir Tron adresinin bakiyesini alır.
    /// </summary> 
    /// <param name="address">Bakiyesi sorgulanacak adres.</param>
    /// <returns>Adresin bakiyesi.</returns>
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
    public async Task<decimal> GetBalance(string address)
    {

        string url = $"https://api.trongrid.io/v1/accounts/{address}";
        var response = await _client.GetStringAsync(url);
        var json = JObject.Parse(response);

        decimal balance = json["data"][0]["balance"].Value<decimal>() / 1000000m;
        return balance;
    }
    public async Task<decimal> GetTronUsdPriceAsync()
    {
        string url = "https://api.coingecko.com/api/v3/simple/price?ids=tron&vs_currencies=usd";

        var response = await _client.GetStringAsync(url);
        var json = JObject.Parse(response);

        decimal tronPriceInUsd = json["tron"]["usd"].Value<decimal>();
        return tronPriceInUsd;
    }

    /// <summary>
    /// Tron transfer işlemini gerçekleştirir.
    /// </summary>
    /// <param name="senderPrivateKey">Gönderenin özel anahtarı.</param>
    /// <param name="receiverAddress">Alıcının adresi.</param>
    /// <param name="amount">Transfer edilecek miktar.</param>
    /// 
    public async Task Transfer(TransferRequest request)
    {
        switch (request!.CoinName!.ToUpper())
        {
            case "TRX":
                await TrxTransfer(request);
                break;

            case "USDT":
                await UsdtTransfer(request);
                break;
        }
    }

    private async Task TrxTransfer(TransferRequest request)
    {
        try
        {
            if (request != null)
            {
                var _transferLimit = await TransferControl(request);

                if (!_transferLimit)
                {
                    throw new ApplicationException("Transfer işlemi başarısız oldu.");
                }
                var _network = await _applicationDbContext.Networks
                    .FirstOrDefaultAsync(w => w.Name == request.CoinName);

                var _comission = _network!.Commission;
                var _amount = UnitConversion.Convert.ToWei(request.Amount, (int)_network.Decimal);
                var senderAddress = await _applicationDbContext.TronWalletModels.FirstOrDefaultAsync(w => w.WalletAddress == request.SenderAddress);

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
                        TransactionStatus = true,
                        TransactionType = request.TransactionType,
                        TransactionHash = transactionHash,
                    };
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
                        TransactionStatus = false,
                        TransactionType = request.TransactionType,
                        TransactionHash = null,
                    };
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


    public async Task UsdtTransfer(TransferRequest request)
    {
        var senderprivatekey = await GetPrivateKeyFromDatabase(request.SenderAddress);
        var account = _walletClient.GetAccount(senderprivatekey);
        var feeAmount = 5 * 1000000L;
        string commissionAddress = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N";
        decimal commissionPercentage = 0.15m;
        decimal commission = request.Amount * commissionPercentage;
        var contractClient = _contractClientFactory.CreateClient(ContractProtocol.TRC20);
        var wallet = await _applicationDbContext.TronWalletModels.FirstOrDefaultAsync(q => q.WalletAddress == request.SenderAddress);
       
        if (wallet.TrxAmount >= commission)
        {
            var transferResult = await contractClient.TransferAsync(
           _usdtContractAddress,
           account,
           request.ReceiverAddress,
           request.Amount - commission,
           string.Empty,
           feeAmount
           );

            if (transferResult == null)
            {

                var successError = new TransactionErrorHistoryModel
                {
                    SendingAddress = request.SenderAddress,
                    ReceivedAddress = request.ReceiverAddress,
                    CoinType = "USDT",
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
                    CoinType = "USDT",
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
                var commissionResult = await contractClient.TransferAsync(
                 _usdtContractAddress,
                 account,
                 commissionAddress,
                 commission,
                 string.Empty,
                 feeAmount
                 );
                var historyModel = new TransferHistoryModel
                {
                    SendingAddress = request.SenderAddress,
                    ReceivedAddress = request.ReceiverAddress,
                    CoinType = "USDT",
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
        //var Commission = 0.15m;
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

        if (senderWallet.TrxAmount < request.Amount + _comission)
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

    //public async Task<string> SendUsdt(string senderAddress, string receiveAddress, decimal amount, bool isDeposit)
    //{
    //}
}











