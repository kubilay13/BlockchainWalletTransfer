using Entities.Models.TronModels;
using Entities.Models.WalletModel;
using Entities.Models.UserModel;
using Entities.Dto;
namespace ETHWalletApi.Services
{
    public interface IEthService
    {
        Task<WalletModel> CreateAccountETHWalletAsync(UserSignUpModel userSignUpModel);
        Task<string> GetPrivateKeyByAddressAsync(string walletAddress);
        Task<string> SendTransactionAsyncETH(TransferRequest request);
        Task<string> SendTransactionAsyncUSDT(withdrawdto request);
        Task<string> UserLogin(UserLoginRequestDto userLoginRequestDto);
    }
}
