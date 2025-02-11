using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShopBaker.Models
{
    public class TypeCakeModel
    {
        [Key]
        public int TypeCakeId { get; set; } // ID của loại bánh

        [Required]
        [StringLength(100)]
        public string? TypeName { get; set; } // Tên loại bánh

        // Liên kết với danh sách các Cake có loại này
        public ICollection<CakeModel>? Cakes { get; set; }
    }
}

