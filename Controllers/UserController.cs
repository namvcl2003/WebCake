
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopBaker.Controllers.Data;
using ShopBaker.Models;
using Newtonsoft.Json;
using ShopBaker.Session;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShopBaker.Controllers
{
    public class UserController : BaseController
    {
        private readonly ShopBakerDbContext _context;
        

        public UserController(ShopBakerDbContext context, SessionCustomer sessionCustomer)
            : base(sessionCustomer) // Khởi tạo BaseController
        {
            _context = context;
        }
        //public UserController(ShopBakerDbContext context)
        //{
        //    _context = context;

        //}

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string Username, string Password)
        {
            // Kiểm tra thông tin đăng nhập
            var user = _context.Users
                .FirstOrDefault(u => u.Username == Username && u.Password == Password);


            if (user != null)
            {
                
                //// Đăng nhập thành công, lưu thông tin vào session
                HttpContext.Session.SetString("UserName", user.Username);
                HttpContext.Session.SetInt32("UserRole", (int)user.Id);
                // Kiểm tra nếu có session chưa tồn tại thì khởi tạo với giá trị ban đầu là 1
                if (HttpContext.Session.GetInt32("LoginCount") == null)
                {
                    HttpContext.Session.SetInt32("LoginCount", 1);
                }
                else
                {
                    // Nếu đã tồn tại, tăng số lượng lên 1
                    int currentCount = HttpContext.Session.GetInt32("LoginCount") ?? 0;
                    HttpContext.Session.SetInt32("LoginCount", currentCount + 1);
                }

                ViewBag.UserSession = user.Id;
                ViewBag.UserName = user.LastName;
                if (ViewBag.UserSession == 1)
                {
                    // Đăng nhập thành công, chuyển hướng đến trang chủ hoặc trang khác
                    return RedirectToAction("Index","Admin");
                }

                else return RedirectToAction("Index", "Home");

            }

            // Thông báo lỗi nếu đăng nhập thất bại
            ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "User");
        }

        //----------------------------------------------------------------------------------------------

        public IActionResult Register()
        {
            var username = GetUserName();
            ViewBag.UserName = username;
            return View();
        }


        [HttpPost]
        public IActionResult Register(UserModel user)
        {
            if (ModelState.IsValid)
            {
                // Lưu thông tin đăng ký của user vào database
                _context.Users.Add(user);
                _context.SaveChanges();

                // Chuyển hướng tới trang đăng nhập hoặc trang chủ
                return RedirectToAction("Login", "User");
            }
            return View(user);
        }

 //----------------------------------------------------------------------------------------------


        // GET: Display the Change Password form
        [HttpGet]
        public IActionResult ChangePassword()
        {
            var username = GetUserName();
            ViewBag.UserName = username;
            return View();
        }

        // POST: Handle the password change
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid || model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "The new password and confirmation do not match.");
                return View(model);
            }

            var username = HttpContext.Session.GetString("UserName");
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                return RedirectToAction("Login", "User"); // Redirect to login if user not found
            }

            if (user.Password != model.CurrentPassword)
            {
                ModelState.AddModelError("", "Current password is incorrect.");
                return View(model);
            }

            // Update the user's password
            user.Password = model.NewPassword;
            _context.SaveChanges();

            ViewBag.Message = "Password changed successfully!";
            return RedirectToAction("Index", "Home"); ;
        }

//----------------------------------------------------------------------------------------------


        // Action để hiển thị form cập nhật thông tin người dùng
        public async Task<IActionResult> UpdateUserInfo()
        {
            // Lấy thông tin người dùng từ session
            var username = GetUserName();
            ViewBag.UserName = username;
            if (string.IsNullOrEmpty(username))
            {
                // Chuyển hướng về trang đăng nhập nếu chưa đăng nhập
                return RedirectToAction("Login");
            }

            // Tìm người dùng trong database
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }

            return View(user);
        }

        // Xử lý POST khi người dùng cập nhật thông tin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserInfo(UserModel model)
        {
            
            // Lấy thông tin người dùng từ session
            var username = GetUserName();

           
            // Tìm người dùng trong database
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }

            // Cập nhật thông tin người dùng
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Address = model.Address;
            user.Address2 = model.Address2;
            user.City = model.City;
            user.Email = model.Email;
            user.Phone = model.Phone;

            // Lưu thay đổi vào database
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            // Lưu thay đổi vào cơ sở dữ liệu
            _context.SaveChanges();

            // Chuyển hướng về trang hồ sơ hoặc trang thành công
            return RedirectToAction("Index", "Home"); // Đổi thành action phù hợp trong trường hợp có trang hồ sơ
        }

        public IActionResult Index()
        {
            var userName = GetUserName(); // Sử dụng phương thức từ BaseController
            ViewBag.UserName = userName; // Lưu tên người dùng vào ViewBag
            ViewBag.UserId = HttpContext.Session.GetInt32("UserRole");
            var users = _context.Users.ToList();
            return View(users);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserbyAD(UserModel model)
        {
            // Tìm người dùng theo Id
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.Id);
            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }

            // Cập nhật thông tin người dùng
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Address = model.Address;
            user.Address2 = model.Address2;
            user.City = model.City;
            user.Email = model.Email;
            user.Phone = model.Phone;

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Chuyển hướng về trang danh sách người dùng hoặc trang phù hợp khác
            return RedirectToAction("Index", "User");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateUserbyAD(int id)
        {
            // Tìm người dùng theo ID
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }

            // Sử dụng ViewBag để truyền username đến View
            ViewBag.Username = user.Username;

            // Trả về view với model là thông tin người dùng
            return View(user);
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }



        [HttpGet]
        public IActionResult GetUserById(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return Json(user);
        }


    }
}

