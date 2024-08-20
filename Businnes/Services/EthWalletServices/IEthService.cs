using Entities.Models.WalletModel;
using Entities.Models.UserModel;
using Entities.Dto;
using Entities.Dto.TronDto;
using Entities.Dto.WalletApiDto;
namespace ETHWalletApi.Services
{
    public interface IEthService
    {
        Task<WalletModel> CreateAccountETHWalletAsync(UserSignUpModel userSignUpModel);
        Task<string> GetPrivateKeyByAddressAsync(string walletAddress);
        Task<string> SendTransactionAsyncETH(TransferRequest request);
        Task<string> SendTransactionAsyncUSDT(EthUsdtDto request);
        Task<string> UserLogin(UserLoginRequestDto userLoginRequestDto);
    }
}
