using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopBaker.Controllers.Data;
using ShopBaker.Session;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopBaker.Models;
using System.Linq;
using System.Globalization;

namespace ShopBaker.Controllers
{

    public class AdminController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ShopBakerDbContext _context;

        public AdminController(ILogger<HomeController> logger, ShopBakerDbContext context, SessionCustomer sessionCustomer)
            : base(sessionCustomer) // Kế thừa từ BaseController
        {
            _logger = logger;
            _context = context;
        }


        public IActionResult Index()
        {
            //Thống kê
            var cakes = _context.Cakes.ToList();
            var customer = _context.Users.ToList();
            var Bill = _context.BillingDetails.ToList();
            ViewBag.totalproduct = cakes.Count();
            ViewBag.totalclients = customer.Count();
            ViewBag.totalbill = Bill.Count();
            ViewBag.totalPrice = _context.BillingDetails.Sum(b => b.Total);
            //-----------------------------------------------------------------
            var userName = GetUserName(); // Sử dụng phương thức từ BaseController
            ViewBag.UserName = userName; // Lưu tên người dùng vào ViewBag
            ViewBag.UserId = HttpContext.Session.GetInt32("UserRole");
            //----------------------------------------------------------------
            //Thống kế theo tháng:
            // Lấy tháng và năm hiện tại
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;
            // Lấy tháng và năm của tháng trước
            var previousMonth = DateTime.Now.AddMonths(-1);
            int previousMonthValue = previousMonth.Month;
            int previousYearValue = previousMonth.Year;

            // Tính tổng của cột Total trong tháng hiện tại
            var totalInCurrentMonth = _context.BillingDetails
                .Where(b => b.TimeOrder.Month == currentMonth && b.TimeOrder.Year == currentYear)
                .Sum(b => (decimal?)b.Total) ?? 0;
            // Tính tổng của cột Total cho tháng trước đó
            var totalInPreviousMonth = _context.BillingDetails
                .Where(b => b.TimeOrder.Month == previousMonthValue && b.TimeOrder.Year == previousYearValue)
                .Sum(b => (decimal?)b.Total) ?? 0;

            // Truy vấn tổng doanh thu của từng tháng
            var monthlyProfits = _context.BillingDetails
                .GroupBy(b => b.TimeOrder.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Total = g.Sum(b => b.Total)
                })
                .OrderBy(m => m.Month)
                .ToList();
            // Kiểm tra dữ liệu trước khi truyền vào ViewBag
            if (monthlyProfits == null || !monthlyProfits.Any())
            {
                Console.WriteLine("No data found for monthly profits.");
            }

            // Ghép dữ liệu với các tháng còn thiếu
            var monthlyProfitsWithAllMonths = monthlyProfits;


            var monthlyRevenue = _context.BillingDetails
             .GroupBy(b => new { b.TimeOrder.Year, b.TimeOrder.Month })
             .Select(g => new
             {
                 Year = g.Key.Year,
                 Month = g.Key.Month,
                 Revenue = g.Sum(b => b.Total)
             })
             .ToList() // Execute the query and bring the data into memory
             .Select(g => new
             {
                 Month = new DateTime(g.Year, g.Month, 1).ToString("MMM yyyy", CultureInfo.InvariantCulture),
                 Revenue = g.Revenue
             })
             .OrderBy(m => m.Month)
             .ToList();

            ViewBag.MonthlyRevenue = monthlyRevenue;

            // Truyền dữ liệu vào View thông qua ViewBag
            ViewBag.MonthlyProfits = monthlyProfitsWithAllMonths;
            // Truyền tổng giá trị ra View
            ViewBag.TotalInCurrentMonth = totalInCurrentMonth;
            ViewBag.TotalInPreviousMonth = totalInPreviousMonth;
            ViewBag.TotalGrowth = ((totalInCurrentMonth - totalInPreviousMonth) / totalInPreviousMonth) * 100;

            ViewBag.LoginCount = HttpContext.Session.GetInt32("LoginCount");



            return View();
        }


        public IActionResult Ecommerce()
        {
            var userName = GetUserName(); // Sử dụng phương thức từ BaseController
            ViewBag.UserName = userName; // Lưu tên người dùng vào ViewBag
            ViewBag.UserId = HttpContext.Session.GetInt32("UserRole");
            ViewBag.LoginCount = HttpContext.Session.GetInt32("LoginCount");
            return View();
        }

        public IActionResult ItemsSales()
        {
            var userName = GetUserName(); // Sử dụng phương thức từ BaseController
            ViewBag.UserName = userName; // Lưu tên người dùng vào ViewBag
            ViewBag.UserId = HttpContext.Session.GetInt32("UserRole");
            return View();
        }

        public IActionResult NewOrders(int page = 1, int pageSize = 10)
        {
            var userName = GetUserName();
            ViewBag.UserName = userName;
            ViewBag.UserId = HttpContext.Session.GetInt32("UserRole");

            int offset = (page - 1) * pageSize;

            var billingDetails = _context.BillingDetails
                .OrderBy(b => b.TimeOrder)
                .Skip(offset)
                .Take(pageSize)
                .Include(b => b.Carts)
                .ToList();

            int totalBills = _context.BillingDetails.Count();
            int totalPages = (int)Math.Ceiling(totalBills / (double)pageSize);

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            return View(billingDetails);
        }
        private const int PageSize = 9; // Items per page
        public async Task<IActionResult> TotalProducts(int page = 1,  string searchQuery = "")
        {
           
            var userName = GetUserName(); // Sử dụng phương thức từ BaseController
            ViewBag.UserName = userName; // Lưu tên người dùng vào ViewBag
            ViewBag.UserId = HttpContext.Session.GetInt32("UserRole");
            // Truy vấn dữ liệu từ CartModel và tính toán thông tin bán hàng
            var cakeSales = _context.Carts
                                    .AsEnumerable() // Chuyển đổi sang client-side để tính toán các trường như TotalPrice
                                    .GroupBy(c => c.ProductName)  // Nhóm theo tên sản phẩm 
                                    .Select(group => new CakeSalesViewModel
                                    {
                                        ProductName = group.Key,
                                        Price = group.FirstOrDefault().Price,  // Lấy giá từ sản phẩm đầu tiên trong nhóm
                                        QuantitySold = group.Sum(g => g.Quantity),  // Tổng số lượng bán ra
                                        TotalRevenue = group.Sum(g => g.TotalPrice),  // Tổng tiền thu được
                                        ImageUrl = group.FirstOrDefault().ImageUrl
                                    })
                                    .ToList();
            int totalItems = cakeSales.Count();
            var paginatedCakes = cakeSales.Skip((page - 1) * PageSize).Take(PageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            return View("TotalProducts", paginatedCakes);
        }

       
        public async Task<IActionResult> AdminCake(int page = 1, string sortOrder = "", decimal? minPrice = null, decimal? maxPrice = null, string searchQuery = "", string category = "")
        {

            ViewBag.UserName = GetUserName();
            ViewBag.UserId = HttpContext.Session.GetInt32("UserRole");
            ViewBag.SearchQuery = searchQuery;
            // Lấy danh sách loại bánh từ cơ sở dữ liệu và truyền qua ViewBag
            var typeCakeList = await _context.TypeCake
                .Select(tc => new { Value = tc.TypeCakeId, Text = tc.TypeName })
                .ToListAsync();
            ViewBag.TypeCakeList = typeCakeList;
            // Get all cakes (you may want to replace this with a call to your database)
            var cakes = _context.Cakes.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                cakes = cakes.Where(c => c.Name.Contains(searchQuery));
            }
            // Filter by category if provided
            if (!string.IsNullOrEmpty(category))
            {
                cakes = cakes.Where(c => c.Name.Contains(category));
            }

            // Áp dụng lọc giá nếu có
            if (minPrice.HasValue && maxPrice.HasValue)
            {
                cakes = cakes.Where(c => c.Price >= minPrice.Value && c.Price <= maxPrice.Value);

            }
            // Sorting logic
            switch (sortOrder)
            {
                case "price_asc":
                    cakes = cakes.OrderBy(c => c.Price);
                    break;
                case "price_desc":
                    cakes = cakes.OrderByDescending(c => c.Price);
                    break;
                default:
                    cakes = cakes.OrderBy(c => c.Name); // Default sorting
                    break;
            }



            // Pagination logic
            int totalItems = cakes.Count();
            var paginatedCakes = cakes.Skip((page - 1) * PageSize).Take(PageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            ViewBag.SortOrder = sortOrder;
            
            return View("AdminCake", paginatedCakes);
        }




        public async Task<IActionResult> Edit(int id)
        {
            var cake = await _context.Cakes.FindAsync(id);
            if (cake == null) return NotFound();

            // Lấy danh sách loại bánh từ cơ sở dữ liệu và truyền qua ViewBag
            var typeCakeList = await _context.TypeCake
                .Select(tc => new { Value = tc.TypeCakeId, Text = tc.TypeName })
                .ToListAsync();
            ViewBag.TypeCakeList = typeCakeList;

            return Json(new { id = cake.Id, name = cake.Name, price = cake.Price, imageUrl = cake.ImageUrl, typeCakeId = cake.TypeCakeId });
        }

        [HttpPost]
        public async Task<IActionResult> EditCake(CakeModel cake)
        {
            if (ModelState.IsValid)
            {
                var existingCake = await _context.Cakes.FindAsync(cake.Id);
                if (existingCake == null) return Json(new { success = false });

                // Cập nhật các thuộc tính
                existingCake.Name = cake.Name;
                existingCake.Price = cake.Price;
                existingCake.TypeCakeId = cake.TypeCakeId;

                // Nếu có file ảnh mới thì cập nhật
                if (Request.Form.Files.Any())
                {
                    var imageFile = Request.Form.Files[0];
                    using var memoryStream = new MemoryStream();
                    await imageFile.CopyToAsync(memoryStream);
                    existingCake.ImageUrl = Convert.ToBase64String(memoryStream.ToArray()); // Giả định lưu ảnh dưới dạng Base64
                }

                _context.Cakes.Update(existingCake);
                await _context.SaveChangesAsync();  

                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

    }
	
}

