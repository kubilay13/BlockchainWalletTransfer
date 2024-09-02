
using CrypticWizard.RandomWordGenerator;
using Entities.Dto.WalletApiDto;
using Solnet.Programs;
using Solnet.Rpc;
using Solnet.Wallet.Utilities;
using Solnet.Wallet;
using Solnet.Wallet.Bip39;
using Solnet.KeyStore;

namespace SolanaWalletApi.Services.SolanaWalletService
{
    public class SolanaWalletService : ISolanaWalletService
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
            var sollet = new Wallet(Solnet.Wallet.Bip39.WordCount.Twelve, WordList.English);
            Console.WriteLine(sollet);
            var account = sollet.GetAccount(10);
            return $"Solana Cüzdan Oluşturma İşlemi Başarılı:\n" +
                     $"Wallet Address: {account}\n" +
                   $"Wallet Key: {sollet.Mnemonic}\n" +
                   $"Wallet Private Key: {sollet.Account.PrivateKey}";
        }

        public async Task<string> SendTransactionSolanaAsync(TransferRequest request)
        {
            var rpcClient = ClientFactory.GetClient(Cluster.TestNet);

            // Get the recent block hash
            var blockHash = await rpcClient.GetRecentBlockHashAsync();

            // Create PublicKey objects
            //string result = privateKey.ToStringByteArray();
            var fromAccount = new Account("jtJA7tinVLVGuiaZJbCuYxH8z968JZQLrLr786Nhxc6z67twF5rywKNEwr3eP1kV2e8L33E376Xo4y4jW3TPv6R", "dokwUS43N6aJENXYzQEs4f1pu1rtQqHHZtbwUrCcZDo");

            // Create the transaction
            var tx = new Solnet.Rpc.Builders.TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(fromAccount)
                .AddInstruction(SystemProgram.Transfer(fromAccount, fromAccount.PublicKey, (ulong)1))
                .Build(fromAccount);

            // Sign the transaction
            var signature = await rpcClient.SendTransactionAsync(tx);

            return $"{signature}";
        }
    }
}
