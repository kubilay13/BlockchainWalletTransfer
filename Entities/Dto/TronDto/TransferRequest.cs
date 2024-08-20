using Entities.Enums;

namespace Entities.Dto.TronDto
{
    public class TransferRequest
    {
        public string? SenderAddress { get; set; }
        public string? ReceiverAddress { get; set; }
        public decimal Amount { get; set; }
        public string? Network { get; set; }
        public string? CoinName { get; set; }
        public TransactionType TransactionType { get; set; }
    }
}
