using DataAccessLayer.AppDbContext;
using Entities.Dto.TronDto;
using Entities.Dto.WalletApiDto;
using Entities.Models.AdminModel;
using Entities.Models.EthModels;
using Entities.Models.UserModel;
using Entities.Models.WalletModel;
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
        [HttpPost("CreateWallet-UserSignUp(ETH,TRX)")]
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
                    var balance = await _tronService.GetBalanceAsyncTron(WalletAdress);
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
            if (request.Network == "TRON")
            {
                if(request.CoinName == "TRX" )
                {
                    await _tronService.TrxTransfer(request);
                    return Ok($" Transfer İşlemi Başarılı. \n{request.CoinName}");
                }
                else if ( request.CoinName == "USDT" || request.CoinName == "USDC" || request.CoinName == "USDD")
                {
                    await _tronService.TokenTransfer(request);
                    return Ok($" Transfer İşlemi Başarılı. \n{request.CoinName}");
                }
            }
            else if (request.Network == "ETH")
            {
                if (request.CoinName == "ETH")
                {
                    await _ethService.SendTransactionAsyncETH(request);
                    return Ok($" Transfer İşlemi Başarılı. \n{request.CoinName}");
                }
                else if ( request.CoinName == "USDT")
                {
                    //await _ethService.SendTransactionAsyncUSDT(request);
                    return Ok($" Transfer İşlemi Başarılı. \n{request.CoinName}");
                }
            }
            else
            {
                throw new NotImplementedException($"İşlem Başarısız. \n{request.CoinName}");
            }
            throw new NotImplementedException($"İşlem Başarılı. \n{request.CoinName}");
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
        [HttpPost("UserLogin")]
        public async Task<IActionResult> UserLogin(UserLoginRequestDto userLoginRequestDto)
        {
            var result = await _tronService.UserLogin(userLoginRequestDto);
            if (result == "Kayıtlı Mail Bulunamadı.")
            {
                return BadRequest(result);
            }
            if (result == "Yanlış şifre girdiniz.")
            {
                return Unauthorized(result); 
            }
            return Ok(result); 
        }
    }
}

