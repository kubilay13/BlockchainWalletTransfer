namespace TronWalletApi.Services.TronWalletService
{
    public interface ITronWalletService
    {
        Task GetNetworkFee();
        Task UpdateWalletAmountsAsync();

        
    }
}
