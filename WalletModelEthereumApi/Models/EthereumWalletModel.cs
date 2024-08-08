namespace WalletModelEthereumApi.Models
{
    public class EthereumWalletModel
    {
        public int Id { get; set; }

        public string? WalletName { get; set; }

        public string? WalletAdress { get; set; }

        public string? PrivateKey { get; set; }

        public decimal ETHAmount { get; set; }
    }
}
