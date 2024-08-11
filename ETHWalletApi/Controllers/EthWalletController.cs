using ETHWalletApi.Models;
using ETHWalletApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Org.BouncyCastle.Asn1.Ocsp;

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
        public class CreateWalletRequest
        {
            public string WalletName { get; set; }
        }
        [HttpGet("CreateETHWallet")]
        public async Task<IActionResult> CreateETHWallet([FromBody] CreateWalletRequest request)
        {
            if (string.IsNullOrEmpty(request.WalletName))
            {
                return BadRequest(new { message = "Cüzdan adı gereklidir." });
            }

            try
            {
                var walletDetails = await _ethService.CreateETHWalletAsync(request.WalletName);

                var returnEthDetail = new
                {
                    PrivateKey = walletDetails.PrivateKey,
                    PublicKey = walletDetails.PublicKey,
                    WalletAddress = walletDetails.WalletAddress,
                    WalletETHScanURL = walletDetails.WalletETHScanURL
                };

                return Ok(returnEthDetail);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Beklenmeyen bir hata oluştu.", error = ex.Message });
            }
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
