using Entities.Models.WalletModel;
using Entities.Models.UserModel;
using Entities.Dto.WalletApiDto;
using Entities.Models.AdminModel;
namespace ETHWalletApi.Services
{
    public interface IEthService
    {
        Task<WalletModel> CreateAccountETHWalletAsync(UserSignUpModel userSignUpModel);
        Task<string> EthTokenTransfer(TransferRequest request);
        Task<string> UserLogin(UserLoginRequestDto userLoginRequestDto);
        Task<string> AdminLogin(AdminLoginModel adminLoginModel);
    }
}
