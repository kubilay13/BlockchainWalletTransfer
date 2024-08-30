namespace Business.Services.TronService
{
    public interface ITronWalletService
    {
        Task GetNetworkFee();
        Task UpdateWalletAmountsAsync();
        Task<decimal> GetBalanceAsyncUsdtBackgroundService(string address, string privatekey);
        Task<decimal> GetBalanceAsyncUsdcBackgroundService(string UsdcBalance, string privatekey);
        Task<decimal> GetBalanceAsyncUsddBackgroundService(string UsdcBalance, string privatekey);
        Task<decimal> GetBalanceAsyncBttBackgroundService(string UsdcBalance, string privatekey);
        Task<decimal> GetBalanceAsyncTrxBackgroundService(string address);
        Task<decimal> GetBalanceAsyncTron(string address);
    }
}
