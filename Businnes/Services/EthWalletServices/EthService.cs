using Business.Services.EthWalletServices.EthTransferService;
using Business.Services.WalletPrivatekeyToPasswords;
using DataAccessLayer.AppDbContext;
using Entities.Dto.WalletApiDto;
using Entities.Models.AdminModel;
using Entities.Models.UserModel;
using Entities.Models.WalletModel;
using Microsoft.EntityFrameworkCore;
using Nethereum.Hex.HexConvertors.Extensions;
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
        private readonly IEthTransferService _ethTransferService;

        public EthService(ApplicationDbContext applicationDbContext, HttpClient httpClient, IWalletPrivatekeyToPassword walletPrivatekeyToPassword,IEthTransferService ethTransferService)
        {
            _applicationDbContext = applicationDbContext;
            _web3 = new Web3("https://sepolia.infura.io/v3/3fcb68529b9e4288a4eb599f266bbb50");
            _httpClient = httpClient;
            _walletPrivatekeyToPassword = walletPrivatekeyToPassword;
            _ethTransferService = ethTransferService;   
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
                byte[] encryptedPrivateKey = _walletPrivatekeyToPassword.EncryptPrivateKey(Convert.ToString(privateKey));
                string base64PrivateKey = Convert.ToBase64String(encryptedPrivateKey);
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
                    PrivateKeyEth = base64PrivateKey,
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
        public async Task TransferETHorToken(TransferRequest request, string transactiontype)
        {
            switch (request!.CoinName!.ToUpper())
            {
                case "ETH":
                    await _ethTransferService.SendTransactionAsyncETH(request);
                    break;

                default:
                    await EthTokenTransfer(request);
                    break;
            }
        }
        public async Task<string> EthTokenTransfer(TransferRequest request)
        {
            if(request.Network=="ETHEREUM" && request.CoinName=="ETH")
            {
                await _ethTransferService.SendTransactionAsyncETH(request);
                return "ETH Transfer İşleminiz Başarılı.";
            }
            else if (request.Network == "ETHEREUM" && request.CoinName == "USDT")
            {
                await _ethTransferService.SendTransactionAsyncUSDT(request);
                return "USDT Transfer İşleminiz Başarılı.";
            }
            else if (request.Network == "ETHEREUM" && request.CoinName == "Bnb")
            {
                await _ethTransferService.SendTransactionAsyncBnb(request);
                return "Bnb Transfer İşleminiz Başarılı.";
            }
            else if (request.Network=="BİNANCE" && request.CoinName=="BNB")
            {
                await _ethTransferService.SendTransactionAsyncBnb_Bnb(request);
                return "Bnb Transfer İşleminiz Başarılı.";
            }
            else
            {
                return "Lütfen Geçerli Transfer İşlemini Girin.";
            }
            
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

