﻿using DataAccessLayer.AppDbContext;
using Entities.Models.AdminModel;
using Entities.Models.TronModels;
using Entities.Models.UserModel;
using Entities.Models.WalletModel;
using ETHWalletApi.Services;
using Microsoft.EntityFrameworkCore;
using Nethereum.Signer;
using Newtonsoft.Json.Linq;
using System.Numerics;
using System.Text;
using TronNet;

namespace WalletsApi.Services
{
    public class WalletServices : IWalletService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly HttpClient _httpClient;
        private readonly string _nodeUrl = "https://sepolia.infura.io/v3/3fcb68529b9e4288a4eb599f266bbb50";
        private readonly string _tronApiUrl = "https://api.trongrid.io";
        private readonly ITronService _tronService;
        private readonly IEthService _ethService;
        public WalletServices(ApplicationDbContext applicationDbContext, HttpClient httpClient,ITronService tronService, IEthService ethService)
        {
            _applicationDbContext = applicationDbContext;
            _httpClient = httpClient;
            _tronService= tronService;
            _ethService= ethService;
        }
        public async Task<string> CreateWallet(UserSignUpModel userSignUpModel)
        {
            try
            {
                var ethKey = EthECKey.GenerateKey();
                var ethprivateKey = ethKey.GetPrivateKeyAsBytes().ToHex();
                var ethaddress = ethKey.GetPublicAddress();

                var ecKey = TronECKey.GenerateKey(TronNetwork.MainNet);
                var privateKey = ecKey.GetPrivateKey();
                var address = ecKey.GetPublicAddress();
                var wallet = new WalletModel
                {
                    Name = userSignUpModel.Name,
                    Surname = userSignUpModel.Surname,
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
                var currency = new WalletDetailModel
                {
                    UserId = wallet.Id,
                    PrivateKeyTron = privateKey,
                    WalletAddressTron = address,
                    PrivateKeyEth = ethprivateKey,
                    WalletAddressETH = ethaddress,
                    TrxAmount = 0,
                    UsdcAmount = 0,
                    UsdtAmount = 0,
                    WalletScanURL = $"https://nile.tronscan.org/#/address/{address}",
                    WalletId = wallet.Id
                };
                _applicationDbContext.WalletDetailModels.Add(currency);
                await _applicationDbContext.SaveChangesAsync();
                var network = await _applicationDbContext.Networks.FirstOrDefaultAsync(n => n.Type == Entities.Enums.NetworkType.Network);
                string adminAddress = network.AdminWallet;
                var responseBuilder = new StringBuilder();
                responseBuilder.AppendLine($"WalletName: {wallet.WalletName}");
                responseBuilder.AppendLine($"Tron Private Key: {currency.PrivateKeyTron}");
                responseBuilder.AppendLine($"Tron Wallet Address: {currency.WalletAddressTron}");
                responseBuilder.AppendLine($"Ethereum Private Key: {currency.PrivateKeyEth}");
                responseBuilder.AppendLine($"Ethereum Wallet Address: {currency.WalletAddressETH}");
                var response = responseBuilder.ToString();
                return response;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Tron cüzdanı oluşturma işlemi başarısız oldu.", ex);
            }
        }
        public async Task Transfer( TransferRequest request, string Network)
        {
            if (request.Network == "TRON")
            {
                if(request.CoinName == "TRX" ||  request.CoinName=="USDT" || request.CoinName=="USDC" || request.CoinName=="USDD")
                {
                      await _tronService.TokenTransfer(request);
                }
            }
            else if (request.Network == "ETH")
            {
                if( request.CoinName == "ETH" || request.CoinName == "USDT")
                {
                    await _ethService.SendTransactionAsyncETH(request);
                }
                
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
    }

}
