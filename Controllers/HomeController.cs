
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShopBaker.Controllers.Data;
using ShopBaker.Models;
using ShopBaker.Session;

namespace ShopBaker.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ShopBakerDbContext _context;

        public HomeController(ILogger<HomeController> logger, ShopBakerDbContext context, SessionCustomer sessionCustomer)
            : base(sessionCustomer) // Kế thừa từ BaseController
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var userName = GetUserName(); // Sử dụng phương thức từ BaseController
            ViewBag.UserName = userName; // Lưu tên người dùng vào ViewBag
            ViewBag.UserId = HttpContext.Session.GetInt32("UserRole");
            // Lấy tất cả các bánh từ bảng Cakes
            var cakes = _context.Cakes.ToList();

            // Debug: Kiểm tra nếu không có bánh nào
            if (cakes == null || !cakes.Any())
            {
                Console.WriteLine("Không có bánh nào trong cơ sở dữ liệu!");
            }

            return View(cakes);
        }

        

        public IActionResult Privacy()
        {
            return View();
        }

        //public IActionResult Admin()
        //{
        //    var userName = GetUserName(); // Sử dụng phương thức từ BaseController
        //    ViewBag.UserName = userName; // Lưu tên người dùng vào ViewBag
        //    ViewBag.UserId = HttpContext.Session.GetInt32("UserRole");

        //    var billingDetails = _context.BillingDetails
        //        .Include(b => b.Carts) // Lấy cả thông tin sản phẩm từ giỏ hàng
        //        .ToList();
        //    return View(billingDetails);
        //}
        //public IActionResult Admin(int page = 1, int pageSize = 10)
        //{
        //    var userName = GetUserName();
        //    ViewBag.UserName = userName;
        //    ViewBag.UserId = HttpContext.Session.GetInt32("UserRole");

        //    int offset = (page - 1) * pageSize;

        //    var billingDetails = _context.BillingDetails
        //        .OrderBy(b => b.TimeOrder)
        //        .Skip(offset)
        //        .Take(pageSize)
        //        .Include(b => b.Carts)
        //        .ToList();

        //    int totalBills = _context.BillingDetails.Count();
        //    int totalPages = (int)Math.Ceiling(totalBills / (double)pageSize);

        //    ViewBag.TotalPages = totalPages;
        //    ViewBag.CurrentPage = page;

        //    return View(billingDetails);
        //}
        public IActionResult Admin(int page = 1, int pageSize = 10, int? statusFilter = null, DateTime? dateFilter = null, string searchInput = null)
        {
            var query = _context.BillingDetails.AsQueryable();
            var userName = GetUserName(); // Sử dụng phương thức từ BaseController
            ViewBag.UserName = userName; // Lưu tên người dùng vào ViewBag
            ViewBag.UserId = HttpContext.Session.GetInt32("UserRole");
            // Tìm kiếm theo tên khách hàng nếu có
            if (!string.IsNullOrEmpty(searchInput))
            {
                query = query.Where(b =>
                    (b.FirstName != null && b.FirstName.Contains(searchInput)) ||
                    (b.LastName != null && b.LastName.Contains(searchInput)));
            }

            // Lọc theo trạng thái nếu có
            if (statusFilter.HasValue)
            {
                query = query.Where(b => b.Status == statusFilter.Value);
            }

            // Lọc theo ngày nếu có
            if (dateFilter.HasValue)
            {
                query = query.Where(b => b.TimeOrder.Date == dateFilter.Value.Date);
            }

            // Tổng số hóa đơn
            int totalBills = query.Count();
            int totalPages = (int)Math.Ceiling(totalBills / (double)pageSize);

            // Lấy dữ liệu phân trang
            var billingDetails = query
                .OrderBy(b => b.TimeOrder) // Sắp xếp theo thời gian đặt hàng
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(b => b.Carts) // Bao gồm giỏ hàng
                .ToList();

            // Truyền dữ liệu vào ViewBag
            ViewBag.StatusFilter = statusFilter;
            ViewBag.DateFilter = dateFilter;
            ViewBag.SearchInput = searchInput;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            return View(billingDetails);
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(int id, int status)
        {
            var order = _context.BillingDetails.Find(id);
            if (order != null)
            {
                Console.WriteLine($"Before Update - Order Status: {order.Status}");
                order.Status = status;
                _context.SaveChanges();
                Console.WriteLine($"After Update - Order Status: {order.Status}");
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        public IActionResult OderStatus()
        {
            var userName = GetUserName(); // Sử dụng phương thức từ BaseController
            ViewBag.UserName = userName; // Lưu tên người dùng vào ViewBag
            ViewBag.UserId = HttpContext.Session.GetInt32("UserRole");


            var userRole = HttpContext.Session.GetInt32("UserRole");
            ViewBag.UserId = userRole;
            var billingDetails = _context.BillingDetails
                .Include(b => b.Carts) // Lấy cả thông tin sản phẩm từ giỏ hàng
                .ToList();
                
            return View(billingDetails);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult SetViewAdminCheck(int value)
        {
            HttpContext.Session.SetInt32("Viewadmincheck", value);
           
            // Redirect đến trang đích sau khi cập nhật
            return RedirectToAction("Index","Home");
        }
    }
}