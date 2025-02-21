using BookBee.Model;
using Microsoft.EntityFrameworkCore;

namespace BookBee.Persistences.Repositories.UserRepository
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly DataContext _context;

        public UserProfileRepository(DataContext context)
        {
            _context = context;
        }

        public UserProfile GetUserProfileById(int id)
        {
            return _context.UserProfiles
                .Include(up => up.UserAccount)
                .FirstOrDefault(up => up.Id == id);
        }

        public UserProfile GetUserProfileByUserId(int userId)
        {
            return _context.UserProfiles
                .Include(up => up.UserAccount)
                .FirstOrDefault(up => up.UserAccountId == userId);
        }

        public UserProfile GetUserProfileByEmail(string email)
        {
            return _context.UserProfiles
                .Include(up => up.UserAccount)
                .FirstOrDefault(up => up.Email == email);
        }

        public void CreateUserProfile(UserProfile userProfile)
        {
            _context.UserProfiles.Add(userProfile);
            _context.SaveChanges();
        }

        public void UpdateUserProfile(UserProfile userProfile)
        {
            _context.UserProfiles.Update(userProfile);
            _context.SaveChanges();
        }

        public void DeleteUserProfile(UserProfile userProfile)
        {
            _context.UserProfiles.Remove(userProfile);
            _context.SaveChanges();
        }

        public bool IsSaveChanges()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
