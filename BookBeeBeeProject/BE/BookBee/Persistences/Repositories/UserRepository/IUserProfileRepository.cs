using BookBee.Model;

namespace BookBee.Persistences.Repositories.UserRepository
{
    public interface IUserProfileRepository
    {
        UserProfile GetUserProfileById(int id);
        UserProfile GetUserProfileByUserId(int userId);
        UserProfile GetUserProfileByEmail(string email);
        void UpdateUserProfile(UserProfile userProfile);
        void DeleteUserProfile(UserProfile userProfile);
        void CreateUserProfile(UserProfile userProfile);
        bool IsSaveChanges();
    }
}
