using Entities.Enums;
using System.ComponentModel.DataAnnotations;
namespace Entities.Models.WalletModel
{
    public class WalletModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string? Name { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string? Surname { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 10)]
        public string? TelNo { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string? Password { get; set; }
        public string? WalletName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastTransactionAt { get; set; }

        public bool TransactionLimit { get; set; }
        public string? Network { get; set; }

        public virtual ICollection<WalletDetailModel> WalletDetails { get; set; }

    }
}
