namespace SolanaWalletApi.Services.SolanaPrivateKeyRandom
{
    public interface ISolanaPrivateKeyRandom
    {
        Task<string> SolanaPrivateKeyRandom();
    }
}
