
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolanaWalletApi.Services.SolanaWalletService;
using Solnet.Wallet;

namespace SolanaWalletApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolanaWalletController : ControllerBase
    {
        
        //private readonly ApplicationDbContext _applicationDbContext;
        private readonly ISolanaWalletService _solanaWalletService;

        public SolanaWalletController(ISolanaWalletService solanaWalletService )
        {
            _solanaWalletService = solanaWalletService;
        }
        [HttpPost("CreateWallet")]
        public async Task<IActionResult> CreateSolanaWallet([FromBody] string WalletName)
        {
            if (WalletName == null || string.IsNullOrEmpty(WalletName))
            {
                return BadRequest("Wallet name is required.");
            }
            string result = await _solanaWalletService.CreateSolanaWalletToSign(WalletName);
            return Ok(result);
        }
    }
}
