using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShopBaker.Models
{
    public class BillingDetailsModel
    {
        public int Id { get; set; } // Khóa chính, đã định nghĩa
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Company { get; set; }
        public string? Address { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? OrderNotes { get; set; }
        public DateTime TimeOrder { get; set; }
        public int Status { get; set; }

        public bool New { get; set; }
        public decimal Total { get; set; }
        //public string? Username { get; set; }
        //public string? Password { get; set; }
        // Khóa ngoại liên kết với người dùng



        public int UserId { get; set; }  // ID của người dùng tạo hóa đơn
        public UserModel? User { get; set; }  // Navigation property để liên kết với người dùng


        // Danh sách sản phẩm đã order (giỏ hàng)
        public List<CartModel> Carts { get; set; } = new List<CartModel>();
    }

    public static class OrderStatus
    {
        public const int Pending = 1;        // Chờ xác nhận
        public const int InDelivery = 2;     // Đang giao hàng
        public const int Delivered = 3;      // Đã giao
        public const int Cancelled = 4;      // Đã huỷ

        public static List<SelectListItem> GetAllStatuses()
        {
            return new List<SelectListItem>
        {
            new SelectListItem("Chờ xác nhận", Pending.ToString()),
            new SelectListItem("Đang giao hàng", InDelivery.ToString()),
            new SelectListItem("Đã giao", Delivered.ToString()),
            new SelectListItem("Đã huỷ", Cancelled.ToString())
        };
        }
    }
}

