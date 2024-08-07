using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using TronWalletApi.Context;
using TronWalletApi.Enums;
public class TransferRequest
{
    public string? SenderAddress { get; set; }
    public string? ReceiverAddress { get; set; }
    public decimal Amount { get; set; }
    public string? CoinName { get; set; }
    public TransactionType TransactionType { get; set; }
}

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
    [HttpGet("createwallet")]
    public async Task<IActionResult> CreateWallet(string walletName)
    {
        try
        {
            var walletAddress = await _tronService.CreateWallet(walletName);
            return Ok(walletAddress);
        }
        catch (Exception ex)
        {
            return BadRequest("Cüzdan oluşturulurken bir hata oluştu: " + ex.Message);
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
    [HttpPost("Transfer(TRX,USDT,USDC)")]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.SenderAddress) || string.IsNullOrEmpty(request.ReceiverAddress) || request.Amount <= 0)
        {
            return BadRequest("Geçersiz transfer isteği.");
        }
        try
        {
            var receiverWallet = await _applicationDbContext.TronWalletModels
                .FirstOrDefaultAsync(w => w.WalletAddress == request.ReceiverAddress);

            await _tronService.TransferTRXorToken(request);
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
