using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShopBaker.Controllers.Data;
using ShopBaker.Session;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShopBaker.Controllers
{
    public class MenuController : BaseController
    {
        private readonly ShopBakerDbContext _context;

        public MenuController(ShopBakerDbContext context, SessionCustomer sessionCustomer)
            : base(sessionCustomer) // Khởi tạo BaseController
        {
            _context = context;
        }


       
        public IActionResult Index()
        {
            var userName = GetUserName(); // Sử dụng phương thức từ BaseController
            ViewBag.UserName = userName;
            return View();
        }
    }
}

