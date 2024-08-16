using Microsoft.AspNetCore.Mvc;
using ETHWalletApi.Services;
using Entities.Models.EthModels;

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
        public async Task<IActionResult> CreateWalletAsync([FromBody] string walletName)
        {
            if (string.IsNullOrEmpty(walletName))
            {
                return BadRequest("Wallet name is required.");
            }

            try
            {
                var wallet = await _ethService.CreateETHWalletAsync(walletName);
                var response = new ResponseEthWallet
                {
                    WalletName = walletName,
                    PrivateKey = wallet.PrivateKey,
                    PublicKey = wallet.PublicKey,
                    WalletAddress = wallet.WalletAddress
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("ETHTransfer")]
        public async Task<IActionResult> SendTransactionAsync([FromBody] EthNetworkTransactionRequest request)
        {
            if (request == null)
            {
                return BadRequest("Transaction request is required.");
            }
            try
            {
                var transactionHash = await _ethService.SendTransactionAsync(request);
                return Ok(new { TransactionHash = transactionHash });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
