using Business.Services.WalletPrivatekeyToPasswords;
using DataAccessLayer.AppDbContext;
using Entities.Dto.EthereumDto;
using Entities.Dto.TronDto;
using Entities.Dto.WalletApiDto;
using Entities.Models.AdminModel;
using Entities.Models.TronModels;
using Entities.Models.UserModel;
using Entities.Models.WalletModel;
using Microsoft.EntityFrameworkCore;
using Nethereum.Contracts.Standards.ERC20.ContractDefinition;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Signer;
using Nethereum.Web3;



namespace ETHWalletApi.Services
{
    public class EthService : IEthService
    {
        private readonly Web3 _web3;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly HttpClient _httpClient;
        private readonly IWalletPrivatekeyToPassword _walletPrivatekeyToPassword;
        public EthService(ApplicationDbContext applicationDbContext, HttpClient httpClient, IWalletPrivatekeyToPassword walletPrivatekeyToPassword)
        {
            _applicationDbContext = applicationDbContext;
            _web3 = new Web3("https://sepolia.infura.io/v3/3fcb68529b9e4288a4eb599f266bbb50");
            _httpClient = httpClient;
            _walletPrivatekeyToPassword = walletPrivatekeyToPassword;
        }
        public async Task<WalletModel> CreateAccountETHWalletAsync(UserSignUpModel userSignUpModel)
        {
            var EthKey = EthECKey.GenerateKey();
            var privateKey = EthKey.GetPrivateKeyAsBytes().ToHex();
            var publicKey = EthKey.GetPubKey().ToHex();
            var address = EthKey.GetPublicAddress();
            if (privateKey == null || publicKey == null || address == null)
            {
                throw new ApplicationException("Cüzdan Oluşturma İşlemi Başarısız.");
            }
            else
            {
                //byte[] encryptedPrivateKey = _walletPrivatekeyToPassword.EncryptPrivateKey(Convert.ToString(privateKey));
                //string base64PrivateKey = Convert.ToBase64String(encryptedPrivateKey);
                var walletDetails = new WalletModel
                {
                    Name = userSignUpModel.Name,
                    Surname = userSignUpModel.Surname,
                    WalletName = userSignUpModel.WalletName,
                };
                var wallet = new WalletModel
                {
                    UserId = userSignUpModel.Id,
                    Name = userSignUpModel.Name,
                    Surname = userSignUpModel.Surname,
                    AccountName = userSignUpModel.AccountName,
                    Email = userSignUpModel.Email,
                    TelNo = userSignUpModel.TelNo,
                    Password = userSignUpModel.Password,
                    WalletName = userSignUpModel.WalletName,
                    CreatedAt = DateTime.UtcNow,
                    LastTransactionAt = DateTime.UtcNow,
                    Network = "Testnet(Nile)"
                };
                _applicationDbContext.WalletModels.Add(wallet);
                await _applicationDbContext.SaveChangesAsync();
                var currency = new WalletDetailModel
                {
                    UserId = wallet.Id,
                    PrivateKeyTron = "null",
                    WalletAddressTron = "null",
                    PrivateKeyEth = privateKey,
                    WalletAddressETH = address,
                    TrxAmount = 0,
                    UsdcAmount = 0,
                    UsdtAmount = 0,
                    UsddAmount = 0,
                    WalletScanURL = $"https://sepolia.etherscan.io/address/{address}",
                    WalletId = wallet.Id,
                };
                _applicationDbContext.WalletDetailModels.Add(currency);
                await _applicationDbContext.SaveChangesAsync();
                try
                {
                    await _applicationDbContext.SaveChangesAsync();
                }
                catch (DbUpdateException dbEx)
                {
                    Console.WriteLine($"Veritabanı güncelleme hatası: {dbEx.InnerException?.Message ?? dbEx.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Beklenmeyen hata: {ex.Message}");
                    throw;
                }
                return walletDetails;
            }
        }

        //public async Task TransferTRXorToken(TransferRequest request, string transactiontype)
        //{
        //    switch (request!.CoinName!.ToUpper())
        //    {
        //        case "TRX":
        //            await SendTransactionAsyncETH(request);
        //            break;

        //        default:
        //            await SendTransactionAsyncUSDT(request);
        //            break;
        //    }
        //}
        public async Task<string> SendTransactionAsyncETH(TransferRequest request)
        {
            var senderAddress = await _applicationDbContext.WalletDetailModels.FirstOrDefaultAsync(w => w.WalletAddressETH == request.SenderAddress);
            byte[] encryptedPrivateKeyBytes = Convert.FromBase64String(senderAddress.PrivateKeyEth);
            string decryptedPrivateKey = _walletPrivatekeyToPassword.DecryptPrivateKey(encryptedPrivateKeyBytes);
            var privateKey = await GetPrivateKeyByAddressAsync(request.SenderAddress);
            var account = new Nethereum.Web3.Accounts.Account(decryptedPrivateKey);
            var web3 = new Web3(account, _web3.Client);
            var amountInWei = Web3.Convert.ToWei(request.Amount);
            var currentNonce = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(account.Address);
            try
            {
                var transaction = new TransactionInput
                {
                    From = request.SenderAddress,
                    To = request.ReceiverAddress,
                    Value = new HexBigInteger(amountInWei),
                    Nonce = currentNonce,
                    GasPrice = await web3.Eth.GasPrice.SendRequestAsync()
                };
                var _gas = await web3.Eth.Transactions.EstimateGas.SendRequestAsync(transaction);
                transaction.Gas = _gas;
                var signature = await web3.TransactionManager.SignTransactionAsync(transaction);
                var txnHash = await web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(signature);

                var EthTransaction = new TransferHistoryModel
                {
                    SendingAddress = request.SenderAddress,
                    ReceivedAddress = request.ReceiverAddress,
                    TransactionHash = txnHash,
                    CoinType = "Ethereum",
                    TransactionNetwork = "ETH",
                    TransactionAmount = request.Amount,
                    TransactionDate = DateTime.UtcNow,
                    Commission = 0,
                    NetworkFee = Convert.ToDecimal(_gas.ToString()),
                    TransactionUrl = $"https://sepolia.etherscan.io/tx/{txnHash}",
                    TransactionStatus = true,
                    TransactionType = 0,
                    Network = "TestNet(Sepolia)"
                };
                _applicationDbContext.TransferHistoryModels.Add(EthTransaction);
                await _applicationDbContext.SaveChangesAsync();
                var transactionUrl = $"https://sepolia.etherscan.io/tx/{txnHash}";
                return ("ETH Transfer İşleminiz Başarılı:  " + transactionUrl);
            }
            catch (RpcResponseException ex)
            {
                throw new InvalidOperationException($"ETH Transfer İşlemi Başarısız: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("ETH Transfer İşleminde Beklenmeyen Bir Hata Oluştu. ", ex);
            }
        }
        public async Task<string> SendTransactionAsyncUSDT(EthUsdtDto request)
        {
            var usdtcontract = "0x2DCe21ca7F38D7Fbb6Bbf86AC11ec7867A510f24";
            byte[] privateKey = Convert.FromBase64String(await GetPrivateKeyByAddressAsync(request!.From!));
            string _privateKey = _walletPrivatekeyToPassword.DecryptPrivateKey(privateKey);

            var account = new Nethereum.Web3.Accounts.Account(_privateKey, Chain.Sepolia);
            var web3 = new Web3(account, "https://sepolia.infura.io/v3/3fcb68529b9e4288a4eb599f266bbb50");
            var amountInWei = Web3.Convert.ToWei(request.Amount, 6);
            var currentNonce = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(account.Address);
            var gasPrice = await web3.Eth.GasPrice.SendRequestAsync();
            try
            {
                var transferHandler = web3.Eth.GetContractTransactionHandler<TransferFunction>();

                var transferFunction = new TransferFunction
                {
                    FromAddress = request.From,
                    To = request.To,
                    Value = amountInWei,
                    Nonce = currentNonce,
                    GasPrice = gasPrice,
                };
                var transferReceipt = await transferHandler.SendRequestAsync(usdtcontract, transferFunction);
                return transferReceipt;
            }
            catch (RpcResponseException ex)
            {
                throw new InvalidOperationException($"ETH Transfer İşlemi Başarısız: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("ETH Transfer İşleminde Beklenmeyen Bir Hata Oluştu.", ex);
            }
        }
        public async Task<string> GetPrivateKeyByAddressAsync(string walletAddress)
        {
            var wallet = await _applicationDbContext.WalletDetailModels.FirstOrDefaultAsync(w => w.WalletAddressETH == walletAddress);
            if (wallet == null || wallet.PrivateKeyEth == null)
            {
                throw new ApplicationException("Cüzdan bulunamadı veya privateKey mevcut değil.");
            }
            return wallet.PrivateKeyEth;
        }
        public async Task<string> AdminLogin(AdminLoginModel adminLoginModel)
        {
            var admin = await _applicationDbContext.AdminLoginModels.SingleOrDefaultAsync(a => a.Username == adminLoginModel.Username);
            if (admin == null)
            {
                return ("Kullanıcı adı bulunamadı.");
            }
            if (admin.Password != adminLoginModel.Password)
            {
                return ("Yanlış şifre.");
            }
            return ("Giriş başarılı.");
        }
        public async Task<string> UserLogin(UserLoginRequestDto userLoginRequestDto)
        {
            var userlogin = await _applicationDbContext.userLoginModels.SingleOrDefaultAsync(a => a.UserMailAdress == userLoginRequestDto.Email);
            if (userlogin == null)
            {
                return ("Kayıtlı Mail Bulunamadı.");
            }
            else
            {
                if (userlogin.Password != userLoginRequestDto.Password)
                {
                    return ("yanlış şifre girdiniz.");
                }
            }
            return ("giriş başarılı hogeldiniz.");
        }
    }
}
