namespace Business.Services.WalletPrivatekeyToPasswords
{
    public interface IWalletPrivatekeyToPassword
    {
        byte[] EncryptPrivateKey(string privateKey);
        string DecryptPrivateKey(byte[] encryptedPrivateKey);
    }
}
