namespace ETHWalletApi.Models
{
    public class ETHWalletModels
    {
        public int Id { get; set; }

        public string? WalletName { get; set; }

        public string? WalletAdress { get; set; }

        public string? PrivateKey { get; set; }

        public decimal EthValue { get; set; }
    }
}
