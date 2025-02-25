using BookBee.DTO.Response;
using BookBee.DTO.User;

namespace BookBee.Services.UserService
{
	public interface IUserService
	{
		Task<ResponseDTO> GetUsers(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID");
		Task<ResponseDTO> GetUserById(int id);
		Task<ResponseDTO> GetPersonalInfo();
		Task<ResponseDTO> GetUserByUsername(string username);
		Task<ResponseDTO> UpdateUser(int id, UserAccountDTO userAccountDTO);
		//Task<ResponseDTO> SelfUpdateUser(UserAccountDTO userAccountDTO);
		Task<ResponseDTO> DeleteUser(int id);
		Task<ResponseDTO> RestoreUser(int id);
		Task<ResponseDTO> CreateUser(UserAccountDTO userAccountDTO);
	}
}
