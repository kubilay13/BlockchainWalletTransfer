using Entities.Dto.WalletApiDto;

namespace Business.Services.TronWalletService.TransferTron
{
    public interface ITransferTron
    {
      
        Task TransferTRXorToken(TransferRequest request, string transactionType);
        Task TrxTransfer(TransferRequest request);
        Task TokenTransfer(TransferRequest request);
    }
}
