using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopBaker.Models
{
	public class CartModel
	{
        public int Id { get; set; } // Mã của sản phẩm
        public string? ProductName { get; set; } // Tên sản phẩm
        public string? ImageUrl { get; set; } // Đường dẫn ảnh
        public decimal Price { get; set; } // Giá của sản phẩm
        public int Quantity { get; set; } // Số lượng sản phẩm
        public decimal TotalPrice => Price * Quantity; // Tổng giá cho sản phẩm

        [ForeignKey("BillingDetails")]
        public int BillingDetailsId { get; set; }  // Khóa ngoại chính xác

        public BillingDetailsModel? BillingDetails { get; set; } // Điều này tạo mối quan hệ
    }
    
}

