﻿using Entities.Dto.EthereumDto;

namespace Entities.Dto.WalletApiDto
{
    public class WalletApiTransferRequestDto
    {
        public TransferRequest TransferRequest { get; set; }
        public string Network { get; set; }
        public string Coin { get; set; }
        public EthNetworkTransactionRequest EthNetworkTransactionRequest { get; set; }
    }
}
