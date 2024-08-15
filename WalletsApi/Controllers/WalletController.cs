using DataAccessLayer.AppDbContext;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Numerics;
using System.Text;
using WalletsApi.Services;

namespace WalletsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly ITronService  _tronService;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly HttpClient _httpClient;
        public WalletController(ApplicationDbContext applicationDbContext,IWalletService walletService, ITronService tronService, HttpClient httpClient)
        {
            _walletService = walletService;
            _applicationDbContext = applicationDbContext;
            _tronService = tronService;
            _httpClient = httpClient;
        }
        [HttpPost("CreateWallet(ETH,TRX)")]
        public async Task<string> CreateWallet(string walletName)
        {
            try
            {
                var wallet = await _walletService.CreateWallet(walletName);
                return wallet;
            }
            catch (Exception ex)
            {
                return "Cüzdan Oluşurken Beklenmedik Hata Oluştu.";
            }
        }
        [HttpPost("WalletBalance(ETH,TRX)")]
        public async Task<decimal> WalletBalance(string WalletAdress)
        {
            try
            {
                if (WalletAdress.StartsWith("0x") && WalletAdress.Length == 42)
                {
                    
                    return await ETHWalletBalance(WalletAdress);
                }
                else
                {
                    var balance = await _tronService.GetBalanceAsync(WalletAdress);
                    return balance;
                };
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        private async Task<decimal> ETHWalletBalance(string address)
        {
            try
            {
                string infuraUrl = "https://sepolia.infura.io/v3/3fcb68529b9e4288a4eb599f266bbb50"; 

                var requestBody = new
                {
                    jsonrpc = "2.0",
                    method = "eth_getBalance",
                    @params = new object[] { address, "latest" },
                    id = 1
                };

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(infuraUrl, content);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                JObject jsonResponse = JObject.Parse(responseBody);

                // "result" alanını kontrol edin
                string balanceHex = jsonResponse["result"]?.ToString();

                if (string.IsNullOrEmpty(balanceHex))
                {
                    Console.WriteLine("Error: Received an empty or null balanceHex.");
                    return 0;
                }

                // Bu noktaya kadar kontrol edin
                BigInteger balanceWei = BigInteger.Parse(balanceHex.TrimStart('0'), System.Globalization.NumberStyles.HexNumber);
                decimal balanceEth = (decimal)balanceWei / 1e18m;
                return balanceEth;
            }
            catch (Exception ex)
            {
                // Hata detaylarını kaydedin
                Console.WriteLine($"Error in ETHWalletBalance: {ex.Message}");
                return 0;
            }
        }

        

    }
}
