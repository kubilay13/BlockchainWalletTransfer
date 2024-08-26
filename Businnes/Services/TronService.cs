﻿using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;
using TronNet;
using HDWallet.Core;
using TronNet.Crypto;
using Newtonsoft.Json;
using Google.Protobuf;
using TronNet.Contracts;
using Transaction = TronNet.Protocol.Transaction;
using Nethereum.Util;
using Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Entities.Models.TronModels;
using Entities.Enums;
using DataAccessLayer.AppDbContext;
using System.Text;
using Entities.Models.WalletModel;
using Entities.Models.UserModel;
using Entities.Models.AdminModel;
using Entities.Dto.TronDto;
using Entities.Dto.WalletApiDto;
using Business.Services.WalletPrivatekeyToPasswords;

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
    public TronService(HttpClient client, ITronClient tronClient, ApplicationDbContext applicationDbContext, IWalletClient walletClient, ITransactionClient transactionClient, HttpClient httpClient, ILogger<TronService> logger, IContractClientFactory contractClientFactory, IConfiguration configuration,IWalletPrivatekeyToPassword walletPrivatekeyToPassword)
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
        _walletPrivatekeyToPassword=walletPrivatekeyToPassword;
    }
    public async Task<string> CreateWalletTRON(UserSignUpModel userSignUpModel)
    {
        try
        {
            var ecKey = TronECKey.GenerateKey(TronNetwork.MainNet);
            var privateKey = ecKey.GetPrivateKey();
            var address = ecKey.GetPublicAddress();
            var wallet = new WalletModel
            {
                UserId=userSignUpModel.Id,
                Name = userSignUpModel.Name,
                Surname = userSignUpModel.Surname,
                AccountName= userSignUpModel.AccountName,
                Email = userSignUpModel.Email,
                TelNo = userSignUpModel.TelNo,
                Password = userSignUpModel.Password,
                WalletName = userSignUpModel.WalletName,
                CreatedAt = DateTime.UtcNow,
                LastTransactionAt = DateTime.UtcNow,
                Network = "Testnet(Nile)"
            };
            _applicationDbContext.WalletModels.Add(wallet);
            await _applicationDbContext.SaveChangesAsync();
            byte[] encryptedPrivateKey = _walletPrivatekeyToPassword.EncryptPrivateKey(Convert.ToString(privateKey));
            string base64PrivateKey = Convert.ToBase64String(encryptedPrivateKey);

            var currency = new WalletDetailModel
            {
                UserId = wallet.Id,
                PrivateKeyTron = base64PrivateKey,
                WalletAddressTron = address,
                PrivateKeyEth = "null",
                WalletAddressETH = "null",
                TrxAmount = 0,
                UsdcAmount = 0,
                UsdtAmount = 0,
                UsddAmount = 0,
                WalletScanURL = $"https://nile.tronscan.org/#/address/{address}",
                WalletId = wallet.Id,
               
            };
            _applicationDbContext.WalletDetailModels.Add(currency);
            await _applicationDbContext.SaveChangesAsync();
            var network = await _applicationDbContext.Networks.FirstOrDefaultAsync(n => n.Type == NetworkType.Network);
            string adminAddress = network.AdminWallet;
            await SendTronAsync(adminAddress, address, 20000000);
            var responseBuilder = new StringBuilder();
            responseBuilder.AppendLine($"WalletName: {wallet.WalletName}");
            responseBuilder.AppendLine($"Tron Private Key: {currency.PrivateKeyTron}");
            responseBuilder.AppendLine($"Tron Wallet Address: {currency.WalletAddressTron}");
            responseBuilder.AppendLine($"ETH Private Key: {currency.PrivateKeyEth}");
            responseBuilder.AppendLine($"ETH Wallet Address: {currency.WalletAddressETH}");
            var response = responseBuilder.ToString();
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
    public async Task<decimal> GetBalanceAsyncTron(string address)
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
    public async Task<List<AssetBalance>> GetAllWalletBalanceAsyncTron(string address)
    {
        try
        {
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
    public async Task TransferTRXorToken(TransferRequest request, string transactiontype)
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
    public async Task TrxTransfer(TransferRequest request)
    {
        //var _transferLimit = await TransferControl(request);

        //if (!_transferLimit)
        //{
        //    throw new ApplicationException("Transfer işlemi başarısız oldu.");
        //}
       try
    {
        if (request != null)
        {
            if (request.SenderAddress == request.ReceiverAddress)
            {
                throw new ApplicationException("Alıcı Cüzdan Adresiyle Gönderici Adres Aynılar.");
            }

            var _network = await _applicationDbContext.Networks.FirstOrDefaultAsync(w => w.Name == request.CoinName);
            var network = await _applicationDbContext.Networks.FirstOrDefaultAsync(n => n.Type == NetworkType.Network && n.Name == request.CoinName);
            var _comission = _network!.Commission;
            var _amount = UnitConversion.Convert.ToWei(request.Amount, (int)_network.Decimal);

            var senderAddress = await _applicationDbContext.WalletDetailModels.FirstOrDefaultAsync(w => w.WalletAddressTron == request.SenderAddress);
            if (senderAddress == null)
            {
                throw new ApplicationException("Gönderici cüzdan adresi bulunamadı.");
            }

            // Şifrelenmiş private key'i çöz
            byte[] encryptedPrivateKeyBytes = Convert.FromBase64String(senderAddress.PrivateKeyTron);
            string decryptedPrivateKey = _walletPrivatekeyToPassword.DecryptPrivateKey(encryptedPrivateKeyBytes);

            if (_amount > 0)
            {
                var transactionClient = _tronClient.GetTransaction();
                var signedTransaction = await transactionClient.CreateTransactionAsync(request.SenderAddress, request.ReceiverAddress, (long)_amount);
                var transactionSigned = _transactionClient.GetTransactionSign(signedTransaction.Transaction, decryptedPrivateKey);
                var result = await _transactionClient.BroadcastTransactionAsync(transactionSigned);

                if (!result.Result)
                {
                    throw new ApplicationException($"Trx transfer işlemi başarısız oldu. Hata mesajı: {result.Message}");
                }

                var transactionHash = GetTransactionHash(transactionSigned);

                var transferHistory = new TransferHistoryModel
                {
                    SendingAddress = request.SenderAddress,
                    ReceivedAddress = request.ReceiverAddress,
                    CoinType = request.CoinName!.ToUpper(),
                    TransactionNetwork = "TRON",
                    TransactionAmount = request.Amount,
                    TransactionDate = DateTime.UtcNow,
                    Commission = _comission,
                    NetworkFee = 0,
                    TransactionUrl = $"https://nile.tronscan.org/#/transaction/{transactionHash}",
                    TransactionStatus = result.Result,
                    TransactionType = request.TransactionType,
                    TransactionHash = transactionHash,
                    Network = request.Network
                };

                Log.Information(result.Result ? "TRX Transfer İşlemi Başarılı." : "TRX Transfer İşlemi Sırasında Bir Sorun Oluştu.");
                _applicationDbContext.TransferHistoryModels.Add(transferHistory);
                await _applicationDbContext.SaveChangesAsync();

                if (transactionHash != null)
                {
                    senderAddress.TrxAmount -= request.Amount;
                    _applicationDbContext.WalletDetailModels.Update(senderAddress);
                    
                    var receiverAddress = await _applicationDbContext.WalletDetailModels.FirstOrDefaultAsync(w => w.WalletAddressTron == request.ReceiverAddress);
                    if (receiverAddress != null)
                    {
                        receiverAddress.TrxAmount += request.Amount;
                        _applicationDbContext.WalletDetailModels.Update(receiverAddress);
                    }
                    await _applicationDbContext.SaveChangesAsync();
                }

                await WalletTokenAdminComission(request);
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
        var wallet = await _applicationDbContext.WalletDetailModels.FirstOrDefaultAsync(q => q.WalletAddressTron == request.SenderAddress);
        decimal commissionPercentage = network.Commission;
        decimal commission = request.Amount - commissionPercentage;
        if (request.CoinName == "USDT")
        {
            await WalletTransferQuery(request, wallet);
            if (wallet.TrxAmount >= commission)
            {
                if (wallet.UsdtAmount != 0)
                {
                    await WalletSaveHistoryUsdt(request);
                }
            }
        }
        else if (request.CoinName == "USDC")
        {
            var contractClient = _contractClientFactory.CreateClient(ContractProtocol.TRC20);
            var account = _walletClient.GetAccount(senderprivatekey);
            if (wallet.TrxAmount >= commission)
            {
                if (wallet.UsdcAmount != 0)
                {
                    await WalletTokenAdminComission(request);
                    await WalletSaveHistoryUsdc(request);
                }
                else
                {
                    throw new ApplicationException("Cüzdanınızın USDC Bakiyesi 0");
                }
            }
        }
        else if (request.CoinName == "USDT")
        {
            var contractClient = _contractClientFactory.CreateClient(ContractProtocol.TRC20);
            var account = _walletClient.GetAccount(senderprivatekey);
            if (wallet.TrxAmount >= commission)
            {
                if (wallet.UsdtAmount != 0)
                {
                    await WalletTokenAdminComission(request);
                    await WalletSaveHistoryUsdc(request);
                }
                else
                {
                    throw new ApplicationException("Cüzdanınızın USDT Bakiyesi 0");
                }
            }
        }
        else if (request.CoinName == "USDD")
        {
            var contractClient = _contractClientFactory.CreateClient(ContractProtocol.TRC20);
            var account = _walletClient.GetAccount(senderprivatekey);
            if (wallet.TrxAmount >= commission)
            {
                if (wallet.UsddAmount != 0)
                {
                    //await WalletTokenAdminComission(request);
                    await WalletSaveHistoryUsdd(request);
                }
                else
                {
                    throw new ApplicationException("Cüzdanınızın USDD Bakiyesi 0");
                }
            }
        }
        else
        {
            throw new ApplicationException("Coin İsimlerini Düzgün Griniz.");
        }
    }
    public async Task<decimal> GetBalanceAsyncUsdtBackgroundService(string UsdtBalance, string privatekey)
    {
        try
        {
            var account = _walletClient.GetAccount(privatekey);
            var protocol = _contractClientFactory.CreateClient(ContractProtocol.TRC20);
            var usdtbalance = await protocol.BalanceOfAsync(_configuration.GetValue<string>("TRONNetworkContract:Usdt"), account);
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
    public async Task<decimal> GetBalanceAsyncUsdcBackgroundService(string UsdcBalance, string privatekey)
    {
        var acount = _walletClient.GetAccount(privatekey);
        var protocol = _contractClientFactory.CreateClient(ContractProtocol.TRC20);
        var usdcbalance = await protocol.BalanceOfAsync(_configuration.GetValue<string>("TRONNetworkContract:Usdc"), acount);
        if (usdcbalance != null)
        {
            return usdcbalance;
        }
        else
        {
            throw new ApplicationException("API ile iletişim sırasında bir hata oluştu.");
        }
    }
    public async Task<decimal> GetBalanceAsyncUsddBackgroundService(string UsdcBalance, string privatekey)
    {
        var acount = _walletClient.GetAccount(privatekey);
        var protocol = _contractClientFactory.CreateClient(ContractProtocol.TRC20);
        var usddbalance = await protocol.BalanceOfAsync(_configuration.GetValue<string>("TRONNetworkContract:Usdd"), acount);
        if (usddbalance != null)
        {
            return usddbalance;
        }
        else
        {
            throw new ApplicationException("API ile iletişim sırasında bir hata oluştu.");
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
    public async Task<decimal> GetTronUsdApiPriceAsync()
    {
        string url = "https://api.coingecko.com/api/v3/simple/price?ids=tron&vs_currencies=usd";
        var response = await _client.GetStringAsync(url);
        var json = JObject.Parse(response);
        decimal tronPriceInUsd = json["tron"]["usd"].Value<decimal>();
        return tronPriceInUsd;
    }
    private async Task WalletSaveHistoryUsdc(TransferRequest request)   
    {
        var network = await _applicationDbContext.Networks.FirstOrDefaultAsync(n => n.Type == NetworkType.Network && n.Name == request.CoinName);
        decimal commissionPercentage = network.Commission;
        var senderprivatekey = await GetPrivateKeyFromDatabase(request.SenderAddress);
        var account = _walletClient.GetAccount(senderprivatekey);
        decimal commission = request.Amount - commissionPercentage;
        var UsdcFeeAmount = 40 * 1000000L;
        var contractClient = _contractClientFactory.CreateClient(ContractProtocol.TRC20);
        var transferResult = await contractClient.TransferAsync(network.Contract, account, request.ReceiverAddress, request.Amount, string.Empty, UsdcFeeAmount);
        if (transferResult == null)
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
                TransactionStatus = true,
                TransactionType = request.TransactionType,
                TransactionHash = transferResult,
                Network = request.Network
            };
            _applicationDbContext.TransferHistoryModels.Add(historyModel);
            await _applicationDbContext.SaveChangesAsync();
        }
        await Task.CompletedTask;
    }
    private async Task WalletSaveHistoryUsdt(TransferRequest request)
    {
        var network = await _applicationDbContext.Networks.FirstOrDefaultAsync(n => n.Type == NetworkType.Network && n.Name == request.CoinName);
        decimal commissionPercentage = network.Commission;
        var senderprivatekey = await GetPrivateKeyFromDatabase(request.SenderAddress);
        var account = _walletClient.GetAccount(senderprivatekey);
        decimal commission = request.Amount - commissionPercentage;
        var UsdtfeeAmount = 5 * 1000000L;
        var contractClient = _contractClientFactory.CreateClient(ContractProtocol.TRC20);
        var transferResult = await contractClient.TransferAsync(network.Contract, account, request.ReceiverAddress, request.Amount, string.Empty, UsdtfeeAmount);
        if (transferResult == null)
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
                TransactionStatus = true,
                TransactionType = request.TransactionType,
                TransactionHash = transferResult,
                Network = request.Network
            };
            _applicationDbContext.TransferHistoryModels.Add(historyModel);
            await _applicationDbContext.SaveChangesAsync();
        }
        await Task.CompletedTask;
    }
    private async Task WalletSaveHistoryUsdd(TransferRequest request)
    {
        var network = await _applicationDbContext.Networks.FirstOrDefaultAsync(n => n.Type == NetworkType.Network && n.Name == request.CoinName);
        /*decimal commissionPercentage = network.Commission*/;
        var senderprivatekey = await GetPrivateKeyFromDatabase(request.SenderAddress);
        var account = _walletClient.GetAccount(senderprivatekey);
        //decimal commission = request.Amount - commissionPercentage;
        var feeAmount = 15 * 1000000L;
        var contractClient = _contractClientFactory.CreateClient(ContractProtocol.TRC20);
        var transferResult = await contractClient.TransferAsync(network.Contract, account, request.ReceiverAddress,Convert.ToDecimal(request.Amount) , string.Empty, feeAmount);
        if (transferResult == null)
        {
            var historyModel = new TransferHistoryModel
            {
                SendingAddress = request.SenderAddress,
                ReceivedAddress = request.ReceiverAddress,
                CoinType = request.CoinName,
                TransactionNetwork = "TRC20",
                TransactionAmount = request.Amount,
                TransactionDate = DateTime.UtcNow,
                //Commission = commissionPercentage,
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
                //Commission = commissionPercentage,
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
        await Task.CompletedTask;
    }
    private async Task WalletTokenAdminComission(TransferRequest request)
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
    private async Task<string> GetPrivateKeyFromDatabase(string senderadress)
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
    //    //if (senderWallet.TrxAmount < request.Amount /*+_comission*/)
    //    //{
    //    //    throw new ApplicationException($"Yetersiz bakiye.Bakiye {request.Amount + _comission} tutarından fazla olmalıdır.");
    //    //}
    //    //var receiverWallet = await _applicationDbContext.WalletDetailModels.FirstOrDefaultAsync(w => w.WalletAddressTron == request.ReceiverAddress);
    //    //return true;
    //}
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
    public async Task<string>UserLogin(UserLoginRequestDto userLoginRequestDto)
    {
        var userlogin=await _applicationDbContext.userLoginModels.SingleOrDefaultAsync(a=>a.UserMailAdress==userLoginRequestDto.Email);
        if(userlogin==null)
        {
            return ("Kayıtlı Mail Bulunamadı.");
        }else
        {
            if (userlogin.Password != userLoginRequestDto.Password)
            {
                return ("yanlış şifre girdiniz.");
            }
        }
        return ("giriş başarılı hogeldiniz.");
    }

}











