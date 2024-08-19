
namespace Entities.Models
{
    public class WalletDetailModel
    {
        public int Id { get; set; }
        public string? PrivateKeyTron { get; set; }
        public string? WalletAddressTron { get; set; }
        public string? PrivateKeyEth { get; set; }
        public string? PublicKeyEth { get; set; }
        public string? WalletAddressETH { get; set; }
        public decimal TrxAmount { get; set; }
        public decimal UsdtAmount { get; set; }
        public decimal UsdcAmount { get; set; }
        public decimal ETHAmount { get; set; }
    }
}
