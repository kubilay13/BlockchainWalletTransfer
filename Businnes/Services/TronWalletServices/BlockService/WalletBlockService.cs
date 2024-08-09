using Microsoft.EntityFrameworkCore;
using TronWalletApi.Context;

namespace Business.Services.TronWalletServices.BlockService
{
    public class WalletBlockService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public WalletBlockService(ApplicationDbContext dbContext)
        {
            _applicationDbContext =dbContext;
        }
        public async Task GetReadBlock()
        {
            var wallets = await _applicationDbContext.TronWalletModels.ToListAsync();

        }
    }
}
