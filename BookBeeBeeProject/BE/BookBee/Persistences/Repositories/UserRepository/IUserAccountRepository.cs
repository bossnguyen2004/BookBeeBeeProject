using BookBee.Model;

namespace BookBee.Persistences.Repositories.UserRepository
{
    public interface IUserAccountRepository
    {
        List<UserAccount> GetUsers(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID", bool includeDeleted = false);
        UserAccount GetUserById(int id);
        UserAccount GetUserByUsername(string username);
        UserAccount GetUserByEmail(string email);
        void UpdateUser(UserAccount user);
        void DeleteUser(UserAccount user);
        void CreateUser(UserAccount user);
        int GetUserCount();
        bool IsSaveChanges();
        int Total { get; }
    }
}
