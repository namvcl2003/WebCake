using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopBaker.Controllers.Data;
using ShopBaker.Models;
using ShopBaker.Session;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShopBaker.Controllers
{
    
    public class BillingDetailsController : BaseController // Kế thừa từ BaseController
    {
        private readonly ShopBakerDbContext _context;

        public BillingDetailsController(ShopBakerDbContext context, SessionCustomer sessionCustomer)
            : base(sessionCustomer) // Khởi tạo BaseController
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userName = GetUserName(); // Sử dụng phương thức từ BaseController
            ViewBag.UserName = userName; // Lưu tên người dùng vào ViewBag

            return View();
        }


    }
}

