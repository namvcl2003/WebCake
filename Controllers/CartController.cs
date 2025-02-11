
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShopBaker.Controllers.Data;
using ShopBaker.Models;
using ShopBaker.Session;
using System.Collections.Generic;
using System.Linq;

namespace ShopBaker.Controllers
{
    

    public class CartController : BaseController // Kế thừa từ BaseController
    {
            private readonly ShopBakerDbContext _context;

            public CartController(ShopBakerDbContext context, SessionCustomer sessionCustomer)
                : base(sessionCustomer) // Khởi tạo BaseController
            {
                _context = context;
            }

            // Tạm lưu các sản phẩm trong giỏ hàng
            private static List<CartModel> cartItems = new List<CartModel>();


        // Lưu trữ tạm thời cho thông tin người mua
        private static BillingDetailsModel? billingDetails;

        // Phương thức để hiển thị trang giỏ hàng
        public IActionResult Cart()
        {
            var userName = GetUserName(); // Sử dụng phương thức từ BaseController
            ViewBag.UserName = userName; // Lưu tên người dùng vào ViewBag

            // Tính tổng giá trị của giỏ hàng
            decimal totalPrice = cartItems.Sum(item => item.TotalPrice);
            ViewBag.TotalPrice = totalPrice;

            return View(cartItems); // Trả về danh sách sản phẩm trong giỏ
        }
        
        // Phương thức thêm sản phẩm vào giỏ hàng
        public IActionResult AddToCart(int id, string productName, string imageUrl, decimal price, int quantity = 1)
        {
            // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
            var existingItem = cartItems.FirstOrDefault(x => x.Id == id);
            if (existingItem != null)
            {
                // Nếu có rồi, chỉ tăng số lượng sản phẩm
                existingItem.Quantity += quantity;
            }
            else
            {
                // Nếu chưa có, tạo sản phẩm mới và thêm vào giỏ
                cartItems.Add(new CartModel
                {
                    Id = id,
                    ProductName = productName,
                    ImageUrl = imageUrl,
                    Price = price,
                    Quantity = quantity
                });
            }

            return RedirectToAction("Cart"); // Sau khi thêm xong, quay lại trang giỏ hàng
        }

        // Phương thức xoá sản phẩm khỏi giỏ hàng
        public IActionResult RemoveFromCart(int id)
        {
            var itemToRemove = cartItems.FirstOrDefault(x => x.Id == id);
            if (itemToRemove != null)
            {
                cartItems.Remove(itemToRemove);
            }
            return RedirectToAction("Cart","Cart");
        }

        // Phương thức xử lý trang Checkout (giả lập)
        public IActionResult Checkout()
        {
            var userName = GetUserName(); // Sử dụng phương thức từ BaseController
            ViewBag.UserName = userName;

            decimal totalPrice = cartItems.Sum(item => item.Price * item.Quantity);
            ViewBag.TotalPrice = totalPrice;

            // Truyền danh sách sản phẩm trong giỏ hàng sang View
            return View(cartItems);
        }

        [HttpPost]
        public IActionResult Checkout(BillingDetailsModel billingDetailsInput)
        {
            // Lấy thông tin người dùng hiện tại từ session
            var username = HttpContext.Session.GetString("UserName");
            var userName = GetUserName(); // Sử dụng phương thức từ BaseController
            ViewBag.UserName = userName;
            // Truy vấn thông tin người dùng từ database dựa trên Username
            var currentUser = _context.Users.FirstOrDefault(u => u.Username == username);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "User"); // Chuyển hướng nếu không tìm thấy người dùng
            }

            int maxBillsId = _context.BillingDetails.Max(bl => (int?)bl.Id) ?? 0;
            // Cập nhật thông tin người dùng và địa chỉ vào billingDetails
            ViewBag.TotalPrice = cartItems.Sum(item => item.Price * item.Quantity);

            billingDetails = new BillingDetailsModel
            {
                //Id = ++maxBillsId,
                UserId = currentUser.Id,
                FirstName = currentUser.FirstName,
                LastName = currentUser.LastName,
                Email = currentUser.Email,
                Phone = currentUser.Phone,
                Address = currentUser.Address, // Lấy địa chỉ từ bảng Users
                Address2 = currentUser.Address2,
                City = currentUser.City,
                OrderNotes = billingDetailsInput.OrderNotes,
                TimeOrder = DateTime.Now,
                Status = 1,
                New = true,
                Total= ViewBag.TotalPrice

            };

            _context.BillingDetails.Add(billingDetails);
            _context.SaveChanges();


            // Retrieve the highest current ID from ListCart, if available
            int maxListCartId = _context.Carts.Max(lc => (int?)lc.Id) ?? 0;

            // Lưu dữ liệu từ cartItems vào bảng Carts
            foreach (var item in cartItems)
            {
                var cartItem = new CartModel
                {
                    //Id = ++maxListCartId, // Increment from the highest current ID
                    ProductName = item.ProductName,
                    ImageUrl = item.ImageUrl,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    BillingDetailsId = billingDetails.Id // Kết nối với billingDetails
                };
                _context.Carts.Add(cartItem);
            }

            _context.SaveChanges();

            // Tính tổng giá trị giỏ hàng và gán vào ViewBag
            ViewBag.CartItems = cartItems;
            
            ViewBag.billingDetails = billingDetails;
            //// Xóa giỏ hàng sau khi hoàn tất đơn hàng
            //cartItems.Clear();

            // Chuyển tới trang xác nhận đơn hàng
            return View("OrderSuccess", billingDetails);
        }
        public IActionResult OrderSuccess()
        {
            // Kiểm tra nếu billingDetails đã được khởi tạo
            if (billingDetails == null)
            {
                return RedirectToAction("Cart", "Cart"); // Chuyển hướng nếu không có thông tin
            }

            return View(billingDetails); // Trả về billingDetails cho view
        }
    }
}