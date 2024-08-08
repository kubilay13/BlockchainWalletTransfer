namespace Business.Services.TronService
{
    public interface ITronWalletService
    {
        Task GetNetworkFee();
        Task UpdateWalletAmountsAsync();
    }
}
