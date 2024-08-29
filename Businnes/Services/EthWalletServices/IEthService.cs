using Entities.Models.WalletModel;
using Entities.Models.UserModel;
using Entities.Dto.TronDto;
using Entities.Dto.WalletApiDto;
using Entities.Dto.EthereumDto;
using Entities.Models.AdminModel;
namespace ETHWalletApi.Services
{
    public interface IEthService
    {
        Task<WalletModel> CreateAccountETHWalletAsync(UserSignUpModel userSignUpModel);
        Task<string> SendTransactionAsyncETH(TransferRequest request);
        Task<string> SendTransactionAsyncUSDT(EthUsdtDto request);
        Task<string> SendTransactionAsyncBnb(EthUsdtDto request);
        Task<string> UserLogin(UserLoginRequestDto userLoginRequestDto);
        Task<string> AdminLogin(AdminLoginModel adminLoginModel);
    }
}
