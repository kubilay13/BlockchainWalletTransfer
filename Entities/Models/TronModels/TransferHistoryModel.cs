using Entities.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.TronModels
{
    public class TransferHistoryModel
    {
        public int Id { get; set; }
        public string? SendingAddress { get; set; }
        public string? ReceivedAddress { get; set; }
        [MaxLength(256)]
        public string? TransactionHash { get; set; }
        public string? CoinType { get; set; }
        public string? TransactionNetwork { get; set; }
        public decimal TransactionAmount { get; set; }
        public DateTime TransactionDate { get; set; }

        [Column(TypeName = "decimal(18,8)")]

        public decimal Commission { get; set; }
        [Column(TypeName = "decimal(18,8)")]
        public decimal NetworkFee { get; set; }
        public string? TransactionUrl { get; set; }
        public bool TransactionStatus { get; set; }
        public TransactionType TransactionType { get; set; }
        public string TransactionTypeString => TransactionType.ToString();
        public string? Network { get; set; }


        //public TransferHistoryModel()
        //{
        //    TransactionDate = DateTime.UtcNow;
        //    TransactionDateTime = DateTime.Now.ToString("HH:mm:ss");
        //    Network = "Testnet(Nile)";
        //}

    }
}
