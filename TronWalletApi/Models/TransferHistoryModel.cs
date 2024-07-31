using TronWalletApi.Enums;
namespace TronWalletApi.Models
{
    public class TransferHistoryModel
    {
        public int Id { get; set; }
        public string? SendingAddress { get; set; }
        public string? ReceivedAddress { get; set; }
        public string? TransactionHash { get; set; }
        public string? CoinType { get; set; }
        public string? TransactionNetwork { get; set; }
        public decimal TransactionAmount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? TransactionDateTime { get; set; }
        public decimal Commission { get; set; }
        public decimal NetworkFee { get; set; }
        public string? SenderTransactionUrl { get; set; }
        public string? ReceiverTransactionUrl { get; set; }
        public string? TransactionUrl { get; set; }
        public  bool  TransactionStatus { get; set; }
        public TransactionType TransactionType { get; set; }
        public string TransactionTypeString => TransactionType.ToString();
        public string? Network { get; set; }




        public TransferHistoryModel()
        {
            TransactionDate = DateTime.UtcNow;
            TransactionDateTime = DateTime.Now.ToString("HH:mm:ss");
            Network = "Testnet(Nile)";
        }
    }
}
