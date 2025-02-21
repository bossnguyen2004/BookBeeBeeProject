using BookBee.Model;
using Microsoft.EntityFrameworkCore;

namespace BookBee.Persistences.Repositories.UserRepository
{
    public class UserAccountRepository : IUserAccountRepository
    {
        private readonly DataContext _dataContext;

        public UserAccountRepository(DataContext context)
        {
            _dataContext = context;
        }
        public int Total { get; set; }

        public void CreateUser(UserAccount user)
        {
            _dataContext.UserAccounts.Add(user);
            _dataContext.SaveChanges();
        }

        public void DeleteUser(UserAccount user)
        {
            _dataContext.Entry(user).State = EntityState.Modified;
        }

        public UserAccount GetUserByEmail(string email)
        {
            return _dataContext.UserAccounts
                               .Include(u => u.Role)
                               .Include(u => u.UserProfile)
                               .FirstOrDefault(u => u.UserProfile.Email == email);
        }

        public UserAccount GetUserById(int id)
        {
            return _dataContext.UserAccounts
                               .Include(u => u.Role)
                               .Include(u => u.UserProfile)
                               .FirstOrDefault(u => u.Id == id);
        }

        public UserAccount GetUserByUsername(string username)
        {
            return _dataContext.UserAccounts
                           .Include(u => u.Role)        
                           .Include(u => u.UserProfile)  
                           .FirstOrDefault(u => u.Username == username);
        }

        public int GetUserCount()
        {
            return _dataContext.UserAccounts.Count(u => !u.IsDeleted);
        }

        public List<UserAccount> GetUsers(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID", bool includeDeleted = false)
        {
            var query = _dataContext.UserAccounts
           .Include(u => u.Role)
           .Include(u => u.UserProfile)
           .AsQueryable();

            if (!includeDeleted)
            {
                query = query.Where(u => !u.IsDeleted);
            }

            if (!string.IsNullOrEmpty(key))
            {
                query = query.Where(u => u.Username.Contains(key) || u.UserProfile.Email.Contains(key));
            }

            // Sắp xếp theo cột
            query = sortBy?.ToLower() switch
            {
                "username" => query.OrderBy(u => u.Username),
                "email" => query.OrderBy(u => u.UserProfile.Email),
                "role" => query.OrderBy(u => u.Role.Name),
                _ => query.OrderBy(u => u.Id),
            };

            Total = query.Count();

            return query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList();
        }

        public bool IsSaveChanges()
        {
            return _dataContext.SaveChanges() > 0;
        }

        public void UpdateUser(UserAccount user)
        {
            //_dataContext.UserAccounts.Update(user);
            //_dataContext.SaveChanges();
            user.Update = DateTime.Now;
            _dataContext.UserAccounts.Update(user);
        }
    }
}
