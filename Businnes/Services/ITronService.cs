using Entities.Dto.TronDto;
using Entities.Dto.WalletApiDto;
using Entities.Models.AdminModel;
using Entities.Models.TronModels;
using Entities.Models.UserModel;
using Entities.Models.WalletModel;
public interface ITronService
{
    Task<string> CreateWalletTRON(UserSignUpModel userSignUpModel);
    Task<decimal> GetBalanceAsyncTron(string address);
    Task<List<AssetBalance>> GetAllWalletBalanceAsyncTron(string address);
    Task<decimal> GetBalanceAsyncUsdtBackgroundService(string address, string privatekey);
    Task<decimal> GetBalanceAsyncUsdcBackgroundService(string UsdcBalance, string privatekey);
    Task<decimal> GetBalanceAsyncTrxBackgroundService(string address);
    Task<decimal> GetTronUsdApiPriceAsync();
    Task SendTronAsync(string senderAddress, string receiverAddress, long amount);
    Task TransferTRXorToken(TransferRequest request,string transactionType);
    Task<TransactionInfoModel> GetTransactionFeeAsync(string transactionHash);
    Task TokenTransfer(TransferRequest request);
    Task TrxTransfer(TransferRequest request);
    Task<string> AdminLogin(AdminLoginModel adminLoginModel);
    Task<string> UserLogin(UserLoginRequestDto userLoginRequestDto);
    
}
