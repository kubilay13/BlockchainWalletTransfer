using System.ComponentModel.DataAnnotations.Schema;
using TronWalletApi.Enums;

namespace TronWalletApi.Models
{
    public class Network
    {
        public int Id { get; set; }

        public NetworkType Type { get; set; }

        public string? NetworkId {  get; set; }

        public string? Networks { get; set; }

        public string? Name { get; set; }

        public string? Contract { get; set; }
        public int Decimal { get; set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal Commission { get; set; }

        public string? AdminWallet { get; set; }

        public string? AdminWalletPrivateKey { get; set; }
    }
}
