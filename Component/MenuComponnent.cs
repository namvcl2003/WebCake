using System;
using Microsoft.AspNetCore.Mvc;

namespace ShopBaker.Component
{
    public class MenuComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            // Dữ liệu tĩnh cho menu
            var menuItems = new List<string>
            {
                "Trang chủ",
                "Sản phẩm",
                "Dịch vụ",
                "Liên hệ"
            };

            return View(menuItems);
        }
    }
}

