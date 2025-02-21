using BookBee.Model;
using BookBee.Persistences;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookBee.Utilities
{
    public class UserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DataContext _context;
        public UserAccessor(IHttpContextAccessor httpContextAccessor, DataContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public int? GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return string.IsNullOrEmpty(userId) ? null : int.Parse(userId);
        }

        public string? GetCurrentUserName()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
        }

        public string? GetCurrentUserEmail()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);
        }

        public string? GetCurrentUserRole()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
        }

        public string? GetCurrentUserIpAddress()
        {
            return _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        }
        public UserAccount? GetCurrentUserAccount()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return null;

            return _context.UserAccounts
                .Include(u => u.Role) // Nếu cần lấy thông tin Role
                .Include(u => u.UserProfile) // Nếu cần lấy thông tin UserProfile
                .FirstOrDefault(u => u.Id == userId);
        }

        // 🔹 Lấy UserProfile từ CSDL
        public UserProfile? GetCurrentUserProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return null;

            return _context.UserProfiles
                .Include(u => u.UserAccount) // Nếu cần lấy thông tin tài khoản
                .FirstOrDefault(u => u.UserAccountId == userId);
        }
    }
}
