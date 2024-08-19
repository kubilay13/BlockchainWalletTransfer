using DataAccessLayer.AppDbContext;
using Entities.Models.AdminModel;
using Entities.Models.EthModels;
using Entities.Models.TronModels;
using Entities.Models.UserModel;
using ETHWalletApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Numerics;
using System.Text;
using WalletsApi.Services;

namespace WalletsApi.Controllers
{
    public class TransferRequestDto
    {
        public TransferRequest TransferRequest { get; set; }
        public string Network { get; set; }
        public string Coin { get; set; }
        public EthNetworkTransactionRequest EthNetworkTransactionRequest { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly ITronService _tronService;
        private readonly IEthService _ethService;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly HttpClient _httpClient;
    
        public WalletController(ApplicationDbContext applicationDbContext, IWalletService walletService, ITronService tronService, HttpClient httpClient, IEthService ethService)
        {
            _ethService = ethService;
            _walletService = walletService;
            _applicationDbContext = applicationDbContext;
            _tronService = tronService;
            _httpClient = httpClient;
        }
        [HttpPost("CreateWallet(ETH,TRX)")]
        public async Task<string> CreateWallet(UserSignUpModel userSignUpModel)
        {
            try
            {
                var wallet = await _walletService.CreateWallet(userSignUpModel);
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
                if (WalletAdress.StartsWith("T"))
                {
                    var balance = await _tronService.GetBalanceAsync(WalletAdress);
                    return balance;
                };
            }
            catch (Exception ex)
            {
                return 0;
            }
            return 0;
        }

        [HttpPost("WalletTransfer(ETH,TRX)")]
        public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
        {
            if (request.Network == "TRX")
            {
                if (request.CoinName == "TRX" || request.CoinName == "USDT" || request.CoinName == "USDC" || request.CoinName == "USDD")
                {
                    await _tronService.TokenTransfer(request);
                    return Ok($"{request.CoinName} Transfer İşlemi Başarılı.");
                }

            }
            else if (request.Network == "ETH")
            {
                if (request.CoinName == "ETH" || request.CoinName == "USDT")
                {
                    await _ethService.SendTransactionAsyncETH(request);
                    return Ok($" Transfer İşlemi Başarılı. \n{request.CoinName}");
                }
            }
            throw new NotImplementedException("asdasda");
        }

        [HttpPost("AdminLogin")]
        public async Task<IActionResult> AdminLogin(AdminLoginModel adminLoginModel)
        {
            var admin = await _applicationDbContext.AdminLoginModels.SingleOrDefaultAsync(a => a.Username == adminLoginModel.Username);
            if (admin == null)
            {
                return BadRequest("Kullanıcı adı bulunamadı.");
            }
            if (admin.Password != adminLoginModel.Password)
            {
                return BadRequest("Yanlış şifre.");
            }
            return Ok("Giriş başarılı.");
        }
    }
}

