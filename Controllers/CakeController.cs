
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopBaker.Models;
using ShopBaker.Controllers.Data; // Giả sử DbContext được đặt trong namespace này
using ShopBaker.Session; // Thêm namespace cho SessionCustomer
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShopBaker.Controllers
{
    public class CakeController : BaseController // Kế thừa từ BaseController
    {
        private readonly ShopBakerDbContext _context;

        public CakeController(ShopBakerDbContext context, SessionCustomer sessionCustomer)
            : base(sessionCustomer) // Khởi tạo BaseController
        {
            _context = context;
        }

        // Lấy danh sách bánh từ cơ sở dữ liệu
        public IActionResult Index()
        {
            var userName = GetUserName(); // Sử dụng phương thức từ BaseController
            ViewBag.UserName = userName; // Lưu tên người dùng vào ViewBag

            // Lấy tất cả các bánh từ bảng Cakes
            var cakes = _context.Cakes.ToList();

            // Debug: Kiểm tra nếu không có bánh nào
            if (cakes == null || !cakes.Any())
            {
                Console.WriteLine("Không có bánh nào trong cơ sở dữ liệu!");
            }

            return View(cakes); // Trả về view và truyền danh sách bánh
        }

        private const int PageSize = 9; // Items per page

        public async Task<IActionResult> Shop(int page = 1, string sortOrder ="" , decimal? minPrice =null, decimal? maxPrice =null, string searchQuery = "", string category = "")
        {
            ViewBag.UserName = GetUserName();
            ViewBag.UserId = HttpContext.Session.GetInt32("UserRole");
            ViewBag.SearchQuery = searchQuery;
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
            // Lấy danh sách loại bánh từ cơ sở dữ liệu và truyền qua ViewBag
            var typeCakeList = await _context.TypeCake
                .Select(tc => new { Value = tc.TypeCakeId, Text = tc.TypeName })
                .ToListAsync();
            ViewBag.TypeCakeList = typeCakeList;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            ViewBag.SortOrder = sortOrder;

            return View("Shop", paginatedCakes);
        }

        // Lấy danh sách bánh từ cơ sở dữ liệu
        public IActionResult AdminCake()
        {
            var userName = GetUserName(); // Sử dụng phương thức từ BaseController
            ViewBag.UserName = userName; // Lưu tên người dùng vào ViewBag

            // Lấy tất cả các bánh từ bảng Cakes
            var cakes = _context.Cakes.ToList();

            // Debug: Kiểm tra nếu không có bánh nào
            if (cakes == null || !cakes.Any())
            {
                Console.WriteLine("Không có bánh nào trong cơ sở dữ liệu!");
            }

            return View(cakes); // Trả về view và truyền danh sách bánh
        }


        public IActionResult AddCake()
        {
            // Lấy danh sách loại bánh và chuyển sang SelectList cho dropdown
            ViewBag.TypeCakeList = new SelectList(_context.TypeCake, "TypeCakeId", "TypeName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCake(CakeModel model, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra nếu có file ảnh được upload
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    // Tạo đường dẫn lưu trữ trong thư mục wwwroot/images/cakes
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/cakes");
                    Directory.CreateDirectory(uploadsFolder); // Tạo thư mục nếu chưa tồn tại

                    // Tạo tên file duy nhất và lưu ảnh
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    // Lưu đường dẫn tương đối vào thuộc tính ImageUrl của model
                    model.ImageUrl = "/images/cakes/" + uniqueFileName;
                }

                _context.Cakes.Add(model); // Thêm model vào DbSet Cakes
                await _context.SaveChangesAsync();
                return RedirectToAction("Shop"); // Chuyển hướng về trang danh sách bánh hoặc trang phù hợp
                //return RedirectToAction("AdminCake","Admin");
            }

            // Nếu không hợp lệ, lấy lại danh sách TypeCake để hiển thị trong view
            ViewBag.TypeCakeList = new SelectList(_context.TypeCake, "TypeCakeId", "TypeName");
            return View(model);
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


        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var cake = await _context.Cakes.FindAsync(id);
            if (cake != null)
            {
                _context.Cakes.Remove(cake);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Shop"); // Chuyển hướng về trang Shop sau khi xoá
        }
    }
}