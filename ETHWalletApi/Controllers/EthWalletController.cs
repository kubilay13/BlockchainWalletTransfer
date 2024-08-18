﻿using Microsoft.AspNetCore.Mvc;
using ETHWalletApi.Services;
using Entities.Models.EthModels;
using Entities.Models.TronModels;

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
                    PrivateKey = wallet.PrivateKeyEth,
                    PublicKey = wallet.PublicKeyEth,
                    WalletAddress = wallet.WalletAddressETH
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("ETHTransfer")]
        public async Task<IActionResult> SendTransactionAsync([FromBody] TransferRequest request)
        {
            if (request == null)
            {
                return BadRequest("Transaction request is required.");
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

        [HttpPost("send-usdt")]
        public async Task<IActionResult> SendUSDTTransaction([FromBody] EthNetworkTransactionRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.FromAddress) || string.IsNullOrEmpty(request.ToAddress) || request.Amount == null)
            {
                return BadRequest("Geçersiz işlem isteği.");
            }

            try
            {
                var transactionHash = await _ethService.SendTransactionAsyncUSDT(request);
                return Ok(new { TransactionHash = transactionHash });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, $"İşlem sırasında bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Beklenmeyen bir hata oluştu: {ex.Message}");
            }
        }
    }
}
