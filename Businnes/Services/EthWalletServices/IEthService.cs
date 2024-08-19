using Entities.Models.EthModels;
using Entities.Models.TronModels;
using Entities.Models.WalletModel;
using Entities.Models.UserModel;
namespace ETHWalletApi.Services
{
    public interface IEthService
    {
        Task<WalletModel> CreateETHWalletAsync(UserSignUpModel userSignUpModel);
        Task<string> GetPrivateKeyByAddressAsync(string walletAddress);
        Task<string> SendTransactionAsyncETH(TransferRequest request);
        Task<string> SendTransactionAsyncUSDT(EthNetworkTransactionRequest request);

    }
}
