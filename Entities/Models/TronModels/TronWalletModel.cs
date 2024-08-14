namespace Entities.Models.TronModels
{
    public class TronWalletModel
    {
        public int Id { get; set; }
        public string? WalletName { get; set; }
        public string? PrivateKey { get; set; }
        public string? WalletAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedAtTime { get; set; }
        public DateTime LastTransactionAt { get; set; }
        public string? LastTransactionTime { get; set; }
        public decimal TrxAmount { get; set; }
        public decimal UsdtAmount { get; set; }
        public decimal UsdcAmount { get; set; }
        public decimal ETHAmount { get; set; }
        public string? Network { get; set; }
        public string? WalletTronScanURL { get; set; }
        public bool TransactionLimit { get; set; }

        public TronWalletModel()
        {
            CreatedAt = DateTime.UtcNow;
            CreatedAtTime = DateTime.Now.ToString("HH:mm:ss");
            LastTransactionAt = DateTime.UtcNow;
            LastTransactionTime = DateTime.Now.ToString("HH:mm:ss");
            TransactionLimit = false;
            Network = "Testnet(Nile)";
        }
    }
}
