namespace Entities.Models.WalletModel
{
    public class WalletDetailModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? PrivateKeyTron { get; set; }
        public string? WalletAddressTron { get; set; }
        public decimal TrxAmount { get; set; }
        public decimal UsdtAmount { get; set; }
        public decimal UsdcAmount { get; set; }
        public decimal UsddAmount { get; set; }
        public decimal BttAmount { get; set; }
        public string? PrivateKeyEth { get; set; }
        public string? WalletAddressETH { get; set; }
        public decimal ETHAmount { get; set; }
        public string? WalletScanURL { get; set; }
        public int WalletId { get; set; }
        public virtual WalletModel Wallet { get; set; }
    }
}
