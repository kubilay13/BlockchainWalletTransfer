using System.Threading.Tasks;

namespace TronWalletApi.BackgroundServices
{
    public interface ITronWalletAmountUpdateService
    {
        Task UpdateWalletAmountsAsync();
    }
}
