namespace Entities.Models.EthModels
{
    public class EthNetworkTransactionRequest
    {
        public string? FromAddress { get; set; }
        public string? ToAddress { get; set; }
        public decimal? Amount { get; set; }
        public string? PrivateKey { get; set; }

        public decimal TransactionAmount { get; set; }
    }
}
