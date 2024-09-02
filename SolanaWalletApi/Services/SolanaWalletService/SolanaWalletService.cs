
using Solnet.Wallet;
using Solnet.Wallet.Bip39;
using System.Xml.Linq;

namespace SolanaWalletApi.Services.SolanaWalletService
{
    public class SolanaWalletService:ISolanaWalletService
    {
        public SolanaWalletService()
        {
            
        }

        public async Task<string> CreateSolanaWalletToSign(string WalletName)
        {
            var sollet = new Wallet("pistol cat elephant zoo plastic art corn giraffe diamond add name fine ", WordList.English);
            var account = sollet.GetAccount(10);
            return $"Solana Cüzdan Oluşturma İşlemi Başarılı: {account}";
        }
        
    }
}
