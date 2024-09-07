using Business.Services.WalletPrivatekeyToPasswords;
using DataAccessLayer.AppDbContext;
using Entities.Models.UserModel;
using Entities.Models.WalletModel;
using Microsoft.EntityFrameworkCore;
using System.Text;
using TronNet;

namespace Business.Services.TronWalletService.CreateWalletTron
{
    public class CreateWalletTron:ICreateWalletTron
    {
        private readonly IWalletPrivatekeyToPassword _walletPrivatekeyToPassword;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ITronService _tronService;
        public CreateWalletTron(ApplicationDbContext applicationDbContext, ITronService tronService, IWalletPrivatekeyToPassword walletPrivatekeyToPassword)
        {
            _applicationDbContext = applicationDbContext;
            _tronService = tronService;
            _walletPrivatekeyToPassword = walletPrivatekeyToPassword;
        }
        public async Task<string> CreateWalletTRON(UserSignUpModel userSignUpModel)
        {
            try
            {
                var ecKey = TronECKey.GenerateKey(TronNetwork.MainNet);
                var privateKey = ecKey.GetPrivateKey();
                var address = ecKey.GetPublicAddress();
                await WalletSignUpSaveTRON(userSignUpModel, privateKey, address);
                return "Kayıt Başarılı.";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Tron cüzdanı oluşturma işlemi başarısız oldu.", ex);
            }
        }
        public async Task<string> WalletSignUpSaveTRON(UserSignUpModel userSignUpModel, string privateKey, string address)
        {
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
            byte[] encryptedPrivateKey = _walletPrivatekeyToPassword.EncryptPrivateKey(Convert.ToString(privateKey));
            string base64PrivateKey = Convert.ToBase64String(encryptedPrivateKey);

            var currency = new WalletDetailModel
            {
                UserId = wallet.Id,
                PrivateKeyTron = base64PrivateKey,
                WalletAddressTron = address,
                PrivateKeyEth = "null",
                WalletAddressETH = "null",
                TrxAmount = 0,
                UsdcAmount = 0,
                UsdtAmount = 0,
                UsddAmount = 0,
                WalletScanURL = $"https://nile.tronscan.org/#/address/{address}",
                WalletId = wallet.Id,

            };
            _applicationDbContext.WalletDetailModels.Add(currency);
            await _applicationDbContext.SaveChangesAsync();
            var network = await _applicationDbContext.Networks.FirstOrDefaultAsync(n => n.Type == Entities.Enums.NetworkType.Network);
            string adminAddress = network.AdminWallet;
            await _tronService.SendTronAsync(adminAddress, address, 20000000);
            var responseBuilder = new StringBuilder();
            responseBuilder.AppendLine($"WalletName: {wallet.WalletName}");
            responseBuilder.AppendLine($"Tron Private Key: {currency.PrivateKeyTron}");
            responseBuilder.AppendLine($"Tron Wallet Address: {currency.WalletAddressTron}");
            responseBuilder.AppendLine($"ETH Private Key: {currency.PrivateKeyEth}");
            responseBuilder.AppendLine($"ETH Wallet Address: {currency.WalletAddressETH}");
            var response = responseBuilder.ToString();
            return response;
        }
       
    }
}
