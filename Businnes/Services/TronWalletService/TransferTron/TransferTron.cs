using Business.Services.WalletPrivatekeyToPasswords;
using DataAccessLayer.AppDbContext;
using Entities.Dto.WalletApiDto;
using Entities.Models.TronModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nethereum.Util;
using TronNet;
namespace Business.Services.TronWalletService.TransferTron
{
    public class TransferTron : ITransferTron
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IWalletPrivatekeyToPassword _walletPrivatekeyToPassword;
        private readonly ILogger<TransferTron> _logger;
        private readonly ITronService _tronService;
        private readonly ITronClient _tronClient;
        private readonly ITransactionClient _transactionClient;
        public TransferTron(ApplicationDbContext applicationDbContext, IWalletPrivatekeyToPassword walletPrivatekeyToPassword, ILogger<TransferTron> logger = null, ITronService tronService = null, ITronClient tronClient = null, ITransactionClient transactionClient = null)
        {
            _applicationDbContext = applicationDbContext;
            _walletPrivatekeyToPassword = walletPrivatekeyToPassword;
            _logger = logger;
            _tronService = tronService;
            _tronClient = tronClient;
            _transactionClient = transactionClient;
        }
        public async Task TransferTRXorToken(TransferRequest request, string transactiontype)
        {
            switch (request!.CoinName!.ToUpper())
            {
                case "TRX":
                    await TrxTransfer(request);
                    break;

                default:
                    await TokenTransfer(request);
                    break;
            }
        }
        public async Task TrxTransfer(TransferRequest request)
        {
            //var _transferLimit = await TransferControl(request);

            //if (!_transferLimit)
            //{
            //    throw new ApplicationException("Transfer işlemi başarısız oldu.");
            //}
            try
            {
                if (request != null)
                {
                    if (request.SenderAddress == request.ReceiverAddress)
                    {
                        throw new ApplicationException("Alıcı Cüzdan Adresiyle Gönderici Adres Aynılar.");
                    }
                    var _network = await _applicationDbContext.Networks.FirstOrDefaultAsync(w => w.Name == request.CoinName);
                    var network = await _applicationDbContext.Networks.FirstOrDefaultAsync(n => n.Type == Entities.Enums.NetworkType.Network && n.Name == request.CoinName);
                    var _comission = _network!.Commission;
                    var _amount = UnitConversion.Convert.ToWei(request.Amount, (int)_network.Decimal);
                    var senderAddress = await _applicationDbContext.WalletDetailModels.FirstOrDefaultAsync(w => w.WalletAddressTron == request.SenderAddress);
                    if (senderAddress == null)
                    {
                        throw new ApplicationException("Gönderici cüzdan adresi bulunamadı.");
                    }
                    byte[] encryptedPrivateKeyBytes = Convert.FromBase64String(senderAddress.PrivateKeyTron);
                    string decryptedPrivateKey = _walletPrivatekeyToPassword.DecryptPrivateKey(encryptedPrivateKeyBytes);
                    if (_amount > 0)
                    {
                        var transactionClient = _tronClient.GetTransaction();
                        var signedTransaction = await transactionClient.CreateTransactionAsync(request.SenderAddress, request.ReceiverAddress, (long)_amount);
                        var transactionSigned = _transactionClient.GetTransactionSign(signedTransaction.Transaction, decryptedPrivateKey);
                        var result = await _transactionClient.BroadcastTransactionAsync(transactionSigned);
                        if (!result.Result)
                        {
                            throw new ApplicationException($"Trx transfer işlemi başarısız oldu. Hata mesajı: {result.Message}");
                        }
                        var transactionHash = _tronService.GetTransactionHash(transactionSigned);
                        var transferHistory = new TransferHistoryModel
                        {
                            SendingAddress = request.SenderAddress,
                            ReceivedAddress = request.ReceiverAddress,
                            CoinType = request.CoinName!.ToUpper(),
                            TransactionNetwork = "TRON",
                            TransactionAmount = request.Amount,
                            TransactionDate = DateTime.UtcNow,
                            Commission = _comission,
                            NetworkFee = 0,
                            TransactionUrl = $"https://nile.tronscan.org/#/transaction/{transactionHash}",
                            TransactionStatus = result.Result,
                            TransactionType = request.TransactionType,
                            TransactionHash = transactionHash,
                            Network = request.Network
                        };
                        _applicationDbContext.TransferHistoryModels.Add(transferHistory);
                        await _applicationDbContext.SaveChangesAsync();
                        if (transactionHash != null)
                        {
                            senderAddress.TrxAmount -= request.Amount;
                            _applicationDbContext.WalletDetailModels.Update(senderAddress);
                            var receiverAddress = await _applicationDbContext.WalletDetailModels.FirstOrDefaultAsync(w => w.WalletAddressTron == request.ReceiverAddress);
                            if (receiverAddress != null)
                            {
                                receiverAddress.TrxAmount += request.Amount;
                                _applicationDbContext.WalletDetailModels.Update(receiverAddress);
                            }
                            await _applicationDbContext.SaveChangesAsync();
                        }
                        await  _tronService.WalletTokenAdminComission(request);
                    }
                    else
                    {

                    }
                }
            }
            catch (ApplicationException ex)
            {
                _logger.LogError($"Transfer işlemi başarısız oldu. Hata mesajı: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Transfer işlemi başarısız oldu. Hata mesajı: {ex.Message}");
                throw new ApplicationException("Bilinmeyen bir hata oluştu.");
            }
        }
        public async Task TokenTransfer(TransferRequest request)
        {
            var network = await _applicationDbContext.Networks.FirstOrDefaultAsync(n => n.Type == Entities.Enums.NetworkType.Network && n.Name == request.CoinName);
            var senderprivatekey = await _tronService.GetPrivateKeyFromDatabase(request.SenderAddress);
            var wallet = await _applicationDbContext.WalletDetailModels.FirstOrDefaultAsync(q => q.WalletAddressTron == request.SenderAddress);
            decimal commissionPercentage = network.Commission;
            decimal commission = request.Amount - commissionPercentage;
            if (request.CoinName == "USDC")
            {

                if (wallet.TrxAmount >= commission)
                {
                    if (wallet.UsdcAmount != 0)
                    {
                        await _tronService.WalletSaveHistoryToken(request);
                        await _tronService.WalletTokenAdminComission(request);
                    }
                    else
                    {
                        throw new ApplicationException("Cüzdanınızın USDC Bakiyesi 0");
                    }
                }
            }
            else if (request.CoinName == "USDT")
            {
                if (wallet.TrxAmount >= commission)
                {
                    if (wallet.UsdtAmount != 0)
                    {
                        await _tronService.WalletSaveHistoryToken(request);
                        await _tronService.WalletTokenAdminComission(request);
                    }
                    else
                    {
                        throw new ApplicationException("Cüzdanınızın USDT Bakiyesi 0");
                    }
                }
            }
            else if (request.CoinName == "USDD")
            {

                if (wallet.TrxAmount >= commission)
                {
                    if (wallet.UsddAmount != 0)
                    {
                        await _tronService.WalletSaveHistoryToken(request);
                        await _tronService.WalletTokenAdminComission(request);
                    }
                    else
                    {
                        throw new ApplicationException("Cüzdanınızın USDD Bakiyesi 0");
                    }
                }
            }
            else if (request.CoinName == "BTT")
            {

                if (wallet.TrxAmount >= commission)
                {
                    if (wallet.BttAmount != 0)
                    {
                        await _tronService.WalletSaveHistoryToken(request);
                        await _tronService.WalletTokenAdminComission(request);
                    }
                    else
                    {
                        throw new ApplicationException("Cüzdanınızın BTT Bakiyesi 0");
                    }
                }
            }
            else
            {
                throw new ApplicationException("Coin İsimlerini Düzgün Griniz.");
            }
        }
    }
}
