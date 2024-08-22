using System.Security.Cryptography;
using System.Text;
namespace Business.Services.WalletPrivatekeyToPasswords
{
    public class WalletPrivatekeyToPassword: IWalletPrivatekeyToPassword
    {
        public byte[] EncryptPrivateKey(string privateKey)
        {
            byte[] privateKeyBytes = Encoding.UTF8.GetBytes(privateKey);
            return ProtectedData.Protect(privateKeyBytes, null, DataProtectionScope.CurrentUser);
        }
        public string DecryptPrivateKey(byte[] encryptedPrivateKey)
        {
            byte[] decryptedBytes = ProtectedData.Unprotect(encryptedPrivateKey, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
