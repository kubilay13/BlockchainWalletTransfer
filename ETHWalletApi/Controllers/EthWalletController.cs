using Microsoft.AspNetCore.Mvc;
using ETHWalletApi.Services;
using Entities.Models.UserModel;
using Entities.Dto.EthereumDto;
using Entities.Dto.WalletApiDto;
using Business.Services.EthWalletServices.EthTransferService;

namespace ETHWalletApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EthController : ControllerBase
    {
        private readonly IEthService _ethService;
        private readonly IEthTransferService _ethTransferService;
        public EthController(IEthService ethService,IEthTransferService ethTransferService)
        {
            _ethTransferService = ethTransferService;
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
        public async Task<IActionResult> SendEthTransactionAsync([FromBody] TransferRequest request)
        {
            if (request == null)
            {
                return BadRequest("İşlem İsteği null");
            }
            try
            {
                if(request.Network=="ETHEREUM" && request.CoinName=="ETH")
                {
                    var transactionHash = await _ethTransferService.SendTransactionAsyncETH(request);
                    return Ok(new { TransactionHash = transactionHash });
                }
                else if(request.Network== "ETHEREUM" && request.CoinName=="USDT")
                {
                    var transactionHash = await _ethTransferService.SendTransactionAsyncUSDT(request);
                    return Ok(new { TransactionHash = transactionHash });
                }
                else if(request.Network == "ETHEREUM" && request.CoinName == "BNB")
                {
                    var transactionHash = await _ethTransferService.SendTransactionAsyncBnb(request);
                    return Ok(new { TransactionHash = transactionHash });
                }
                else if (request.Network == "BİNANCE" && request.CoinName == "USDT")
                {
                    var transactionHash = await _ethTransferService.SendTransactionAsyncBnbUSDT(request);
                    return Ok(new { TransactionHash = transactionHash });
                }

                return BadRequest("Transfer İşlemi Başarısız.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
       
    }
}
