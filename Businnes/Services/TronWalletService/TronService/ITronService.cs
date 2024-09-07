using Entities.Dto.TronDto;
using Entities.Dto.WalletApiDto;
using Entities.Models.AdminModel;
using Entities.Models.TronModels;
using TronNet.Protocol;
public interface ITronService
{
    Task WalletSaveHistoryToken(TransferRequest request);
    Task<string> GetPrivateKeyFromDatabase(string senderadress);
    Task WalletTokenAdminComission(TransferRequest request);
    Task<TransactionInfoModel> GetTransactionFeeAsync(string transactionHash);
    Task<decimal> GetTronUsdApiPriceAsync();
    Task SendTronAsync(string senderAddress, string receiverAddress, long amount);
    string GetTransactionHash(Transaction signedTransaction);
    Task<string> UserLogin(UserLoginRequestDto userLoginRequestDto);
    Task<string> AdminLogin(AdminLoginModel adminLoginModel);
    Task<List<AssetBalance>> GetAllWalletBalanceAsyncTron(string address, byte[] encryptedPrivateKey);
}
