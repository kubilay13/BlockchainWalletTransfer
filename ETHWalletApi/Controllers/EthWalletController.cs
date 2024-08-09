using ETHWalletApi.Models;
using ETHWalletApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETHWalletApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EthWalletController : ControllerBase
    {
        private readonly IEthService _ethService;

        public EthWalletController(IEthService ethService)
        {
            _ethService = ethService;
        }

        [HttpPost("sendtransaction")]
        public async Task<IActionResult> SendTransactionAsync([FromBody] EthNetworkTransactionRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request");
            }
            try
            {
                // Burada `await` ve `SendTransactionAsync` metodunun çağrıldığından emin olun
                var txnHash = await _ethService.SendTransactionAsyncs(request);
                return Ok(new { TransactionHash = txnHash });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
