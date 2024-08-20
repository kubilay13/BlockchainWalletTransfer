using Entities.Models.EthModels;
using Entities.Models.TronModels;

namespace Entities.Dto
{
    public class WalletApiTransferRequestDto
    {
        public TransferRequest TransferRequest { get; set; }
        public string Network { get; set; }
        public string Coin { get; set; }
        public EthNetworkTransactionRequest EthNetworkTransactionRequest { get; set; }
    }
}
