using System;
using System.ComponentModel.DataAnnotations;

namespace ShopBaker.Models
{
	public class UserModel
	{
		
            [Key]
            public int Id { get; set; }

            [Required]
            [StringLength(100)]
            public string? FirstName { get; set; }

            [Required]
            [StringLength(100)]
            public string? LastName { get; set; }

            [Required]
            [EmailAddress]
            [StringLength(100)]
            public string? Email { get; set; }

            [Required]
            [Phone]
            [StringLength(50)]
            public string? Phone { get; set; }

            [Required]
            [StringLength(100)]
            public string? Username { get; set; }

            [Required]
            [StringLength(255)]
            public string? Password { get; set; }

        // Địa chỉ mới thêm
        public string? Address { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
    }

    public class ChangePasswordViewModel
    {
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}

