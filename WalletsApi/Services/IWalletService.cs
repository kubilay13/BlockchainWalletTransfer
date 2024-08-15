using Microsoft.AspNetCore.Mvc;

namespace WalletsApi.Services
{
    public interface IWalletService
    {
        Task<string> CreateWallet(string walletName);
        Task<decimal>WalletBalance(string WalletAdress);
    }
}
