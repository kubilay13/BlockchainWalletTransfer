using Entities.Enums;
namespace Entities.Models.WalletModel
{
    public class WalletModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? WalletName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastTransactionAt { get; set; }
        public string? WalletScanURL { get; set; }
        public bool TransactionLimit { get; set; }
        public string? Network { get; set; }

        public virtual ICollection<WalletDetailModel> WalletDetails { get; set; }

    }
}
