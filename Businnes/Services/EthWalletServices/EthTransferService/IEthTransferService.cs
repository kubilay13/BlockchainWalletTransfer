﻿using Entities.Dto.WalletApiDto;

namespace Business.Services.EthWalletServices.EthTransferService
{
    public interface IEthTransferService
    {
        Task<string> SendTransactionAsyncETH(TransferRequest request);
        Task<string> SendTransactionAsyncUSDT(TransferRequest request);
        Task<string> SendTransactionAsyncBnb(TransferRequest request);
        Task<string> SendTransactionAsyncBnbBnb(TransferRequest request);
        Task<string> SendTransactionAsyncBnbUSDT(TransferRequest request);
    }
}
