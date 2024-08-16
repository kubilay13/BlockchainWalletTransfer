namespace Entities.Models.TronModels
{
    public class WalletModel
    {
        public int Id { get; set; }

        
        public string? WalletName { get; set; }
        public string? PrivateKeyTron { get; set; }
        public string? WalletAddressTron { get; set; }
        public string? PrivateKeyEth { get; set; }
        public string? WalletAddressETH { get; set; }
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

        public WalletModel()
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
