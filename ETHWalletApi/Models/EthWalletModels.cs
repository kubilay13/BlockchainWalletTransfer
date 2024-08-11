namespace ETHWalletApi.Models
{
    public class EthWalletModels
    {
        public int Id { get; set; }
        public string? WalletName { get; set; }
        public string? PrivateKey { get; set; }
        public string? PublicKey { get; set; }
        public string? WalletAddress { get; set; }
        public decimal ETHAmount { get; set; }
        public string? Network { get; set; }
        public string? WalletETHScanURL { get; set; }
    }
}
