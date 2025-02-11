using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShopBaker.Models;
using System;
using System.Linq;
using ShopBaker.Controllers.Data;
using ShopBaker.Session;

namespace ShopBaker.Controllers
{
    public class ChatController : BaseController
    {
        private readonly ILogger<ChatController> _logger;
        private readonly ShopBakerDbContext _context;

        public ChatController(ILogger<ChatController> logger, ShopBakerDbContext context, SessionCustomer sessionCustomer)
            : base(sessionCustomer)
        {

            _logger = logger;
            _context = context;
        }

        [HttpPost]
        public IActionResult SendMessage([FromBody] ChatMessageDto messageDto)
        {
            var senderId = HttpContext.Session.GetInt32("UserRole");

            if (senderId == null)
            {
                return Json(new { error = "User not logged in" });
            }

            var newMessage = new ChatModel
            {
                SenderId = senderId.Value,
                ReceiverId = messageDto.ReceiverId,
                MessageContent = messageDto.MessageContent,
                SentAt = DateTime.Now
            };
            _context.ChatModels.Add(newMessage);

            // Kiểm tra xem SaveChanges có thành công không
            _logger.LogInformation("Saving message: SenderId={0}, ReceiverId={1}, Content={2}",
                      newMessage.SenderId, newMessage.ReceiverId, newMessage.MessageContent);
            var saveResult = _context.SaveChanges();
            if (saveResult == 0)
            {
                return Json(new { error = "Message was not saved. Please try again." });
            }
            return Json(new { success = true });
        }

            public IActionResult Chatbox()
        {
            return View();
        }

        public IActionResult ChatRoom()
        {
            var userId = HttpContext.Session.GetInt32("UserRole") ?? 0;

            // Lấy danh sách tin nhắn cho cuộc trò chuyện giữa người dùng hiện tại và tất cả người dùng khác
            var messages = _context.ChatModels
                .Where(m => m.SenderId == userId && m.SenderId ==-1)
                .OrderByDescending(m => m.SentAt)
                .ToList();

            // Lấy danh sách người dùng
            var userName = GetUserName(); // Sử dụng phương thức từ BaseController
            ViewBag.UserName = userName; // Lưu tên người dùng vào ViewBag
            
            var users = _context.Users.ToList();
            ViewBag.Users = users;
            ViewBag.UserId = userId;
            ViewBag.Allclient = false;
            ViewBag.Messages = messages;
            
            return View();
        }

        [HttpGet]
        public IActionResult LoadChatMessages(int receiverId)
        {
            ViewBag.ReceiverId = receiverId;
            var userId = HttpContext.Session.GetInt32("UserRole");

            if (userId == null)
            {
                return Json(new { error = "User not logged in" });
            }

            // Thêm dòng này để kiểm tra
            Console.WriteLine($"UserId: {userId}, ReceiverId: {receiverId}");

            var messages = _context.ChatModels
                .Where(m => (m.SenderId == userId && m.ReceiverId == receiverId) ||
                            (m.SenderId == receiverId && m.ReceiverId == userId))
                .OrderBy(m => m.SentAt)
                .Select(m => new {
                    m.MessageContent,
                    m.SentAt,
                    m.SenderId
                })
                .ToList();

            return Json(messages);
        }

        public IActionResult ChatClient()
        {
            var userId = HttpContext.Session.GetInt32("UserRole") ?? 0;

            // Lấy danh sách tin nhắn cho cuộc trò chuyện giữa người dùng hiện tại và tất cả người dùng khác
            var messages = _context.ChatModels
                .Where(m => m.SenderId == userId && m.ReceiverId ==1)
                .OrderByDescending(m => m.SentAt)
                .ToList();

            // Lấy danh sách người dùng
            var users = _context.Users.ToList();
            var userName = GetUserName(); // Sử dụng phương thức từ BaseController
            ViewBag.UserName = userName; // Lưu tên người dùng vào ViewBag
           
            ViewBag.Users = users;
            ViewBag.UserId = userId;
            ViewBag.Allclient = false;
            ViewBag.Messages = messages;
            return View();
        }
    }
}
