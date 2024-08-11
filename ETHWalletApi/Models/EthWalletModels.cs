namespace ETHWalletApi.Models
{
    public class EthWalletModels
    {
        public int Id { get; set; }
        public string? WalletName { get; set; }
        public string? PrivateKey { get; set; }
        public string? WalletAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedAtTime { get; set; }
        public DateTime LastTransactionAt { get; set; }
        public string? LastTransactionTime { get; set; }
        public decimal ETHAmount { get; set; }
        public string? Network { get; set; }
        public string? WalletTronScanURL { get; set; }
        public bool TransactionLimit { get; set; }
    }
}
