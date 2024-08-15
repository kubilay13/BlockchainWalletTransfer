using DataAccessLayer.AppDbContext;
using Entities.Models.TronModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nethereum.JsonRpc.Client;
using Nethereum.Signer;
using Newtonsoft.Json.Linq;
using System.Numerics;
using System.Text;
using TronNet;
using TronNet.Protocol;

namespace WalletsApi.Services
{
    public class WalletServices : IWalletService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly HttpClient _httpClient;
        private readonly string _nodeUrl = "https://sepolia.infura.io/v3/3fcb68529b9e4288a4eb599f266bbb50";
        private readonly string _tronApiUrl = "https://api.trongrid.io";
        public WalletServices(ApplicationDbContext applicationDbContext, HttpClient httpClient)
        {
            _applicationDbContext = applicationDbContext;
            _httpClient = httpClient;
        }
        public async Task<string> CreateWallet(string walletName)
        {
            try
            {
                var ethKey = EthECKey.GenerateKey();
                var ethprivateKey = ethKey.GetPrivateKeyAsBytes().ToHex();
                var ethaddress = ethKey.GetPublicAddress();

                var ecKey = TronECKey.GenerateKey(TronNetwork.MainNet);
                var privateKey = ecKey.GetPrivateKey();
                var address = ecKey.GetPublicAddress();
                var wallet = new TronWalletModel
                {
                    WalletName = walletName,
                    PrivateKeyTron = privateKey,
                    WalletAddressTron = address,
                    PrivateKeyEth = ethprivateKey,
                    WalletAddressETH = ethaddress,
                    CreatedAt = DateTime.UtcNow,
                    CreatedAtTime = DateTime.Now.ToString("HH:mm:ss"),
                    WalletTronScanURL = $"https://nile.tronscan.org/#/address/{address}",
                    Network = "Testnet(Nile)"
                };
                _applicationDbContext.TronWalletModels.Add(wallet);
                await _applicationDbContext.SaveChangesAsync();
                var network = await _applicationDbContext.Networks.FirstOrDefaultAsync(n => n.Type == Entities.Enums.NetworkType.Network);
                string adminAddress = network.AdminWallet;
                var responseBuilder = new StringBuilder();
                responseBuilder.AppendLine($"WalletName: {wallet.WalletName}");
                responseBuilder.AppendLine($"Tron Private Key: {wallet.PrivateKeyTron}");
                responseBuilder.AppendLine($"Tron Wallet Address: {wallet.WalletAddressTron}");
                responseBuilder.AppendLine($"Ethereum Private Key: {wallet.PrivateKeyEth}");
                responseBuilder.AppendLine($"Ethereum Wallet Address: {wallet.WalletAddressETH}");
                var response = responseBuilder.ToString();
                return response;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Tron cüzdanı oluşturma işlemi başarısız oldu.", ex);
            }
        }

        public async Task<decimal> WalletBalance(string WalletAdress)
        {
            if (WalletAdress.StartsWith("0x") && WalletAdress.Length == 42)
            {
                await ETHWalletBalance(WalletAdress);
                return WalletAdress.Length;
            }
            else if (WalletAdress.StartsWith("T") && WalletAdress.Length == 34)
            {
                string url = $"https://nile.trongrid.io/v1/accounts/{WalletAdress}";
                var response = _httpClient.GetStringAsync(url);
                var json = JObject.Parse(response.ToString());
                var balance = json["data"][0]["balance"].Value<decimal>() / 1000000;
                return balance;
            }
            else
            {
                throw new ApplicationException("Balance Sorgusu Yapılırken Bir Sorunla Karşılaşıldı.");
            }
        }
        private async Task<decimal> ETHWalletBalance(string WalletAdress)
        {
            string infuraUrl = "https://sepolia.infura.io/v3/3fcb68529b9e4288a4eb599f266bbb50";
            if (WalletAdress.StartsWith("0x") && WalletAdress.Length == 42)
            {
                var jsonRpcRequest = new
                {
                    jsonrpc = "2.0",
                    method = "eth_getBalance",
                    @params = new object[] { WalletAdress, "latest" },
                    id = 1
                };
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(jsonRpcRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(infuraUrl, content);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                JObject jsonResponse = JObject.Parse(responseBody);
                string balanceHex = jsonResponse["result"].ToString();

                BigInteger balanceWei = BigInteger.Parse(balanceHex.TrimStart('0'), System.Globalization.NumberStyles.HexNumber);
                decimal balanceEth = (decimal)balanceWei / 1e18m;
                return balanceEth;
            }
            else
            {
                throw new ApplicationException("hata");
            }
        }
    }

}
