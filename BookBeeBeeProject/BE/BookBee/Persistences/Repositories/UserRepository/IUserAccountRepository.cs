using BookBee.DTO.Response;
using BookBee.Model;

namespace BookBee.Persistences.Repositories.UserRepository
{
    public interface IUserAccountRepository
    {
		Task<List<UserAccount>> GetAllUserAccount(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID");
		Task<UserAccount> GetUserAccountById(int id);
		Task<ResponseDTO> CreateUserAccount(UserAccount user);
		Task<ResponseDTO> UpdateUserAccount(int id, UserAccount user);
		Task<ResponseDTO> DeleteUserAccount(int id);
		Task<int> GetUserAccountCount();
		Task<bool> IsSaveChanges();
		Task<UserAccount> GetUserByEmail(string email);
		Task<UserAccount> GetUserByUsername(string username);
		//Task<int> GetCurrentUserId();

		int Total { get; }
	}
}
