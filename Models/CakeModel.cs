using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopBaker.Models
{
        public class CakeModel
        {
            public int Id { get; set; } // Để định danh bánh
            public string? Name { get; set; } // Tên bánh
            public decimal Price { get; set; } // Giá bánh
            public string? ImageUrl { get; set; } // Đường dẫn đến hình ảnh bánh


        // Khóa ngoại liên kết với bảng TypeCake
        public int TypeCakeId { get; set; }
      

        // Thuộc tính điều hướng đến TypeCake
        [ForeignKey("TypeCakeId")]
        public TypeCakeModel? TypeCake { get; set; }
    }
    public class CakeSalesViewModel
    {
        public string? ProductName { get; set; }
        public decimal Price { get; set; }
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public string ImageUrl { get; set; }
    }

}

