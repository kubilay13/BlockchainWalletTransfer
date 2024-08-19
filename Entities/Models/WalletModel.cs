using Entities.Enums;
namespace Entities.Models.TronModels
{
    public class WalletModel
    {
        public int Id { get; set; }
        public string? WalletName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastTransactionAt { get; set; }
        public string? WalletScanURL { get; set; }
        public bool TransactionLimit { get; set; }

        public string? Network { get; set; }

        //public WalletModel()
        //{
        //    CreatedAt = DateTime.UtcNow;
        //    CreatedAtTime = DateTime.Now.ToString("HH:mm:ss");
        //    LastTransactionAt = DateTime.UtcNow;
        //    TransactionLimit = false;
        //}
    }
}
