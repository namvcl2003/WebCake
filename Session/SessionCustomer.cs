using System;
using Microsoft.AspNetCore.Http;
namespace ShopBaker.Session
{
    

    public class SessionCustomer
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionCustomer(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserName()
        {
            return _httpContextAccessor.HttpContext.Session.GetString("UserName");
        }

        public bool IsAdmin()
        {
            // Giả sử bạn có lưu trữ quyền admin trong session
            return _httpContextAccessor.HttpContext.Session.GetString("IsAdmin") == "true";
        }

        public void SetUserName(string userName)
        {
            _httpContextAccessor.HttpContext.Session.SetString("UserName", userName);
        }

        public void SetAdmin(bool isAdmin)
        {
            _httpContextAccessor.HttpContext.Session.SetString("IsAdmin", isAdmin.ToString());
        }

        //public int GetUserRole()
        //{
        //    return _httpContextAccessor.HttpContext.Session.SetInt32("UserId");
        //}
    }
}


