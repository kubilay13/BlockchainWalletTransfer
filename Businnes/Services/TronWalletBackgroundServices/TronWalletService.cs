using Business.Services.TronService;
using DataAccessLayer.AppDbContext;
using HDWallet.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using TronNet;
using TronNet.Contracts;
using TronNet.Crypto;
namespace TronWalletApi.Services.TronWalletService
{
    public class TronWalletService : ITronWalletService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ITronService _tronService;
        private readonly ILogger<TronWalletService> _logger;
        private readonly IWalletClient _walletClient;
        private readonly IContractClientFactory _contractClientFactory;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;
        public TronWalletService(ITronService tronService, ApplicationDbContext applicationDbContext, ILogger<TronWalletService> logger,IWalletClient walletClient, IContractClientFactory contractClientFactory,IConfiguration configuration,HttpClient client)
        {
            _tronService = tronService;
            _applicationDbContext = applicationDbContext;
            _logger = logger;
            _walletClient = walletClient;
            _contractClientFactory = contractClientFactory;
            _configuration = configuration;
            _client = client;
          
        }
        public async Task GetNetworkFee()
        {
            var transferHistories = await _applicationDbContext.TransferHistoryModels
                .Where(th => th.NetworkFee == 0)
                .ToListAsync();
            foreach (var transferHistory in transferHistories)
            {
                bool feeUpdated = false;
                while (!feeUpdated)
                {
                    try
                    {
                        var transactionFee = await _tronService.GetTransactionFeeAsync(transferHistory.TransactionHash!);

                        if (transactionFee.Receipt != null)
                        {
                            if (transactionFee?.Fee != null)
                            {

                                transferHistory.NetworkFee = transactionFee.Fee / 1000000;

                                _applicationDbContext.TransferHistoryModels.Update(transferHistory);
                                await _applicationDbContext.SaveChangesAsync();
                                feeUpdated = true;
                            }
                            else
                            {
                                _logger.LogWarning($"NetUsage is non-positive: {transactionFee.Receipt.NetUsage}");
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Receipt is null in transactionFee.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"İşlem ücreti güncellenirken bir hata oluştu. TransactionHash: {transferHistory.TransactionHash}, Hata: {ex.Message}");
                        await Task.Delay(TimeSpan.FromSeconds(10));
                    }
                }
            }
        }
        public async Task UpdateWalletAmountsAsync()
        {
            _logger.LogInformation("Wallet miktarları güncelleniyor.");
            try
            {
                var wallets = await _applicationDbContext.WalletDetailModels.ToListAsync();
                if (wallets != null)
                {
                    foreach (var wallet in wallets)
                    {
                        try
                        {
                            var balance = await GetBalanceAsyncTron(wallet.WalletAddressTron!);
                            wallet.TrxAmount =Convert.ToDecimal(balance);

                            var UsdtBalance = await GetBalanceAsyncUsdtBackgroundService(wallet.WalletAddressTron, wallet.PrivateKeyTron);
                            wallet.UsdtAmount = UsdtBalance;

                            var UsdcBalance = await GetBalanceAsyncUsdcBackgroundService(wallet.WalletAddressTron, wallet.PrivateKeyTron);
                            wallet.UsdcAmount = UsdcBalance;

                            var UsddBalance = await GetBalanceAsyncUsddBackgroundService(wallet.WalletAddressTron, wallet.PrivateKeyTron);
                            wallet.UsddAmount = UsddBalance;

                            var BttBalance = await GetBalanceAsyncBttBackgroundService(wallet.WalletAddressTron, wallet.PrivateKeyTron);
                            wallet.BttAmount = BttBalance;

                            var transferHistories = await _applicationDbContext.TransferHistoryModels
                                .Where(th => th.NetworkFee == 0)
                                .ToListAsync();
                            foreach (var transferHistory in transferHistories)
                            {
                                try
                                {
                                    var transactionFee = await _tronService.GetTransactionFeeAsync(transferHistory.TransactionHash!);

                                    if (transactionFee.Receipt != null)
                                    {
                                        if (transactionFee?.Fee != null)
                                        {
                                            transferHistory.NetworkFee = transactionFee.Fee / 1000000;
                                            await _applicationDbContext.SaveChangesAsync();

                                            _logger.LogInformation("Transfer Ücreti Güncellendi");
                                        }
                                        else
                                        {
                                            _logger.LogWarning($"NetUsage is non-positive: {transactionFee.Receipt.NetUsage}");
                                        }
                                    }
                                    else
                                    {
                                        _logger.LogWarning("Receipt is null in transactionFee.");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError($"İşlem ücreti güncellenirken bir hata oluştu. TransactionHash: {transferHistory.TransactionHash}, Hata: {ex.Message}");
                                    await Task.Delay(TimeSpan.FromSeconds(10));
                                }
                            }
                            _applicationDbContext.WalletDetailModels.Update(wallet);
                            await _applicationDbContext.SaveChangesAsync();
                            _logger.LogInformation("Cüzdan bakiyesi ve NetworkFee başarıyla güncellendi.");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Cüzdan bakiyesi güncellenirken bir hata oluştu. Cüzdan Adresi: {wallet.WalletAddressTron}");
                        }
                    }

                    _logger.LogInformation("Wallet miktarları başarıyla güncellendi.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Wallet miktarları güncellenirken bir hata oluştu.");
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
        public async Task<decimal> GetBalanceAsyncUsddBackgroundService(string UsddBalance, string privatekey)
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
        public async Task<decimal> GetBalanceAsyncBttBackgroundService(string BttBalance, string privatekey)
        {
            var acount = _walletClient.GetAccount(privatekey);
            var protocol = _contractClientFactory.CreateClient(ContractProtocol.TRC20);
            var bttbalance = await protocol.BalanceOfAsync(_configuration.GetValue<string>("TRONNetworkContract:Btt"), acount);
            if (bttbalance != null)
            {
                return bttbalance;
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
    }
}
