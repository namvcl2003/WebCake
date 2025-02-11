using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShopBaker.Session;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShopBaker.Controllers
{
    public class BaseController : Controller
    {
        protected readonly SessionCustomer _sessionCustomer;

        public BaseController(SessionCustomer sessionCustomer)
        {
            _sessionCustomer = sessionCustomer;
        }

        protected bool IsAdmin()
        {
            return _sessionCustomer.IsAdmin();
        }

        protected string GetUserName()
        {
            return _sessionCustomer.GetUserName();
        }

       
    }
}

