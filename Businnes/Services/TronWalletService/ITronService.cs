using Entities.Dto.TronDto;
using Entities.Dto.WalletApiDto;
using Entities.Models.AdminModel;
using Entities.Models.TronModels;
using Entities.Models.UserModel;
public interface ITronService
{
    Task<string> CreateWalletTRON(UserSignUpModel userSignUpModel);
    Task<TransactionInfoModel> GetTransactionFeeAsync(string transactionHash);
    Task<List<AssetBalance>> GetAllWalletBalanceAsyncTron(string address);
    Task<decimal> GetTronUsdApiPriceAsync();
    Task SendTronAsync(string senderAddress, string receiverAddress, long amount);
    Task TransferTRXorToken(TransferRequest request, string transactionType);
    Task TokenTransfer(TransferRequest request);
    Task TrxTransfer(TransferRequest request);
    Task<string> UserLogin(UserLoginRequestDto userLoginRequestDto);
    Task<string> AdminLogin(AdminLoginModel adminLoginModel);
}
