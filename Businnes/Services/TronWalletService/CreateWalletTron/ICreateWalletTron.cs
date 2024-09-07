using Entities.Models.UserModel;

namespace Business.Services.TronWalletService.CreateWalletTron
{
    public interface ICreateWalletTron
    {
        Task<string> WalletSignUpSaveTRON(UserSignUpModel userSignUpModel, string privateKey, string address);
        Task<string> CreateWalletTRON(UserSignUpModel userSignUpModel);
    }
}
