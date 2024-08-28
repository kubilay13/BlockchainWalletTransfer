using Microsoft.AspNetCore.Mvc;
using ETHWalletApi.Services;
using Entities.Models.UserModel;
using Entities.Dto.TronDto;
using Entities.Dto.EthereumDto;

namespace ETHWalletApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EthController : ControllerBase
    {
        private readonly IEthService _ethService;

        public EthController(IEthService ethService)
        {
            _ethService = ethService;
        }

        [HttpPost("CreateETHWallet")]
        public async Task<IActionResult> CreateWalletAsync([FromBody] UserSignUpModel userSignUpModel)
        {
            if (userSignUpModel.Name==null || userSignUpModel.Surname==null || userSignUpModel.AccountName==null || userSignUpModel.Email==null || userSignUpModel.TelNo==null || userSignUpModel.Password==null || userSignUpModel.WalletName==null)
            {
                return BadRequest("Boş Alan Bırakmayınız.");
            }
            else
            {
                try
                {
                    var walletDetails = await _ethService.CreateAccountETHWalletAsync(userSignUpModel);
                    return Ok($"Cüzdan Başarılı Şekilde Oluşturuldu. Cüzdan Adı: {userSignUpModel.WalletName}");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
        }

        [HttpPost("ETHTransfer")]
        public async Task<IActionResult> SendTransactionAsync([FromBody] TransferRequest request)
        {
            if (request == null)
            {
                return BadRequest("İşlem İsteği null");
            }
            try
            {
                var transactionHash = await _ethService.SendTransactionAsyncETH(request);
                return Ok(new { TransactionHash = transactionHash });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //[HttpPost("ETH-USDT-TRANSFER")]
        //public async Task<IActionResult> SendUSDTTransaction([FromBody] EthUsdtDto request)
        //{
        //    if (request == null || string.IsNullOrEmpty(request.SenderAdress) || string.IsNullOrEmpty(request.ReceiverAdress) || request.Amount == null)
        //    {
        //        return BadRequest("Geçersiz işlem isteği.");
        //    }

        //    try
        //    {
        //        var transactionHash = await _ethService.SendTransactionAsyncUSDT(request);
        //        return Ok(new { TransactionHash = transactionHash });
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        return StatusCode(500, $"İşlem sırasında bir hata oluştu: {ex.Message}");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Beklenmeyen bir hata oluştu: {ex.Message}");
        //    }
        //}
    }
}
