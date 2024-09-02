
using CrypticWizard.RandomWordGenerator;
using DataAccessLayer.AppDbContext;
using Solnet.Wallet;
using Solnet.Wallet.Bip39;
using System.Xml.Linq;

namespace SolanaWalletApi.Services.SolanaWalletService
{
    public class SolanaWalletService:ISolanaWalletService
    {
        //private readonly ApplicationDbContext _applicationDbContext;
        private readonly WordGenerator _wordGenerator;
        public SolanaWalletService(/*ApplicationDbContext applicationDbContext*/WordGenerator wordGenerator)
        {
            _wordGenerator = wordGenerator;
            //_applicationDbContext = applicationDbContext;
        }

        public async Task<string> CreateSolanaWalletToSign(string WalletName)
        {
            var generator = _wordGenerator.GetWords(12);
            var sollet = new Wallet(WordCount.Twelve, WordList.English);
            Console.WriteLine(sollet);
            var account = sollet.GetAccount(10);
            return $"Solana Cüzdan Oluşturma İşlemi Başarılı:\n" +
                     $"Wallet Address: {account}\n" +
                   $"Wallet Key: {sollet.Mnemonic}\n" +
                   $"Wallet Private Key: {sollet.Account.PrivateKey}";
        }
    }
}
