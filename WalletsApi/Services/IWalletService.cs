using Entities.Models.AdminModel;
using Entities.Models.UserModel;
using Microsoft.AspNetCore.Mvc;

namespace WalletsApi.Services
{
    public interface IWalletService
    {
        Task<string> CreateWallet(UserSignUpModel userSignUpModel);
        Task<decimal>WalletBalance(string WalletAdress);
        Task<string> AdminLogin(AdminLoginModel adminLoginModel);
    }
}
