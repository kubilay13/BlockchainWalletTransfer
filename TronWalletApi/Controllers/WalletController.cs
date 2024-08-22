using DataAccessLayer.AppDbContext;
using Entities.Dto.TronDto;
using Entities.Enums;
using Entities.Models.UserModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class WalletController : ControllerBase
{
    private readonly ITronService _tronService;
    private readonly ApplicationDbContext _applicationDbContext;

    public WalletController(ITronService tronService, ApplicationDbContext applicationDbContext)
    {
        _tronService = tronService;
        _applicationDbContext = applicationDbContext;
    }
    [HttpPost("CreateWallet-SignUp(TRX Network)")]
    public async Task<IActionResult> CreateWallet( UserSignUpModel userSignUpModel)
    {
        try
        {
            string walletResponse = await _tronService.CreateWalletTRON(userSignUpModel);
            return Ok(new
            {
                Success = true,
                Message = "Cüzdan başarıyla oluşturuldu.",
                WalletInfo = walletResponse
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = "Cüzdan oluşturulurken beklenmedik bir hata oluştu.",
                Error = ex.Message
            });
        }
    }
    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance([FromQuery] string address)
    {
        if (string.IsNullOrEmpty(address))
        {
            return BadRequest("Adres gerekli.");
        }
        try
        {
            var balance = await _tronService.GetBalanceAsync(address);
            var trxUsdPrice = await _tronService.GetTronUsdApiPriceAsync();
            var balanceInUsd = balance * trxUsdPrice;

            return Ok(new
            {
                address,
                balance = $"{balance} TRX",
                balanceInUsd = $"{balanceInUsd} USD"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Sunucu hatası: {ex.Message}");
        }
    }
    [HttpPost("Transfer(TRX,USDT,USDC,USDD)")]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
    {
        if (request == null ||
            string.IsNullOrEmpty(request.SenderAddress) ||
            string.IsNullOrEmpty(request.ReceiverAddress) ||
            request.Amount <= 0)
        {
            return BadRequest("Geçersiz transfer isteği.");
        }
        try
        {
            var receiverWallet = await _applicationDbContext.WalletDetailModels.FirstOrDefaultAsync(w => w.WalletAddressTron == request.ReceiverAddress);

            var transactionType = receiverWallet != null
                ? TransactionType.Deposit
                : TransactionType.Withdraw;

            await _tronService.TransferTRXorToken(request, transactionType.ToString());

            return Ok("Transfer işlemi başarılı.");
        }
        catch (ArgumentException ex)
        {
            return BadRequest($"Geçersiz giriş: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(500, $"İşlem sırasında bir hata oluştu: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Sunucu hatası: {ex.Message}");
        }
    }


    //[HttpPost("TokenTransfer")]
    //public async Task<IActionResult> TokenTransfer([FromBody] TransferRequest request)
    //{
    //    try
    //    {
    //        var usdt =  _tronService.TokenTransfer(request); 
    //        return Ok(usdt);
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest("USDT transferi sırasında bir hata oluştu: " + ex.Message);
    //    }
    //}
}
