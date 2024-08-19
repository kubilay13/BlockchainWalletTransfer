﻿using System.ComponentModel.DataAnnotations;

namespace Entities.Models.UserModel
{
    public class UserSignUpModel
    {
        public int Id { get; set; }
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
    }
}