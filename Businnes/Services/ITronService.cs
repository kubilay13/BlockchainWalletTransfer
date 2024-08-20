﻿using Entities.Dto;
using Entities.Models.AdminModel;
using Entities.Models.TronModels;
using Entities.Models.UserModel;
using Entities.Models.WalletModel;
public interface ITronService
{
    Task<string> CreateWalletTRON(UserSignUpModel userSignUpModel);
    Task<decimal> GetBalanceAsync(string address);
    Task<decimal> GetBalanceAsyncUsdt(string address, string privatekey);
    Task<decimal> GetBalanceAsyncUsdc(string UsdcBalance, string privatekey);
    Task<decimal> GetTronUsdApiPriceAsync();
    Task<decimal> GetBalanceAsyncTrx(string address);
    Task SendTronAsync(string senderAddress, string receiverAddress, long amount);
    Task TransferTRXorToken(TransferRequest request,string transactionType);
    Task<TransactionInfoModel> GetTransactionFeeAsync(string transactionHash);
    Task TokenTransfer(TransferRequest request);
    Task TrxTransfer(TransferRequest request);
    Task<string> AdminLogin(AdminLoginModel adminLoginModel);
    Task<string> UserLogin(UserLoginRequestDto userLoginRequestDto);
}
