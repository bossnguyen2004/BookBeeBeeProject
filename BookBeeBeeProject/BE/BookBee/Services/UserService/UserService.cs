using AutoMapper;
using BookBee.DTO.Response;
using BookBee.DTO.User;
using BookBee.DTO.Voucher;
using BookBee.Model;
using BookBee.Persistences.Repositories.UserRepository;
using BookBee.Persistences.Repositories.VoucherRepository;
using BookBee.Utilities;

namespace BookBee.Services.UserService
{
	public class UserService : IUserService
	{
		private readonly IUserAccountRepository _userAccountRepository;
		private readonly IMapper _mapper;
		private readonly UserAccessor _userAccessor;
		public UserService(IUserAccountRepository userAccountRepository, UserAccessor userAccessor, IMapper mapper)
        {
			_userAccountRepository = userAccountRepository;
			_mapper = mapper;
			 _userAccessor = userAccessor;
		}
        public async Task<ResponseDTO> CreateUser(UserAccountDTO userAccountDTO)
		{
			var existingUser = await _userAccountRepository.GetUserByUsername(userAccountDTO.Username);
			if (existingUser != null)
				return new ResponseDTO { Code = 400, Message = "Tên đăng nhập đã tồn tại" };

			var existingEmail = await _userAccountRepository.GetUserByEmail(userAccountDTO.Email);
			if (existingEmail != null)
				return new ResponseDTO { Code = 400, Message = "Email đã được sử dụng" };

			using var hmac = new System.Security.Cryptography.HMACSHA512();
			var passwordSalt = hmac.Key;
			var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(userAccountDTO.Password));

			var user = new UserAccount
			{
				FirstName = userAccountDTO.FirstName,
				LastName = userAccountDTO.LastName,
				Email = userAccountDTO.Email,
				Gender = userAccountDTO.Gender,
				Phone = userAccountDTO.Phone,
				Dob = userAccountDTO.Dob,
				Avatar = userAccountDTO.Avatar,
				Username = userAccountDTO.Username,
				PasswordSalt = passwordSalt,
				PasswordHash = passwordHash,
				RoleId =  userAccountDTO.RoleId,
				Cart = new Cart(),
				IsVerified = false,
				IsDeleted = false,
				Create = DateTime.Now,
				Update = DateTime.Now
			};

			await _userAccountRepository.CreateUserAccount(user);
			if (await _userAccountRepository.IsSaveChanges())
				return new ResponseDTO { Code = 200, Message = "Tạo tài khoản thành công" };
			else
				return new ResponseDTO { Code = 400, Message = "Tạo tài khoản thất bại" };
		}



		public async Task<ResponseDTO> DeleteUser(int id)
		{
			var user = await _userAccountRepository.GetUserAccountById(id);
			if (user == null)
				return new ResponseDTO { Code = 400, Message = "Người dùng không tồn tại" };

			user.IsDeleted = true;
			await _userAccountRepository.UpdateUserAccount(id, user);
			bool isSaved = await _userAccountRepository.IsSaveChanges();
			return new ResponseDTO { Code = isSaved ? 200 : 400, Message = isSaved ? "Xóa thành công" : "Xóa thất bại" };
		}




		public async Task<ResponseDTO> GetPersonalInfo()
		{
			throw new NotImplementedException();
		}

		public async Task<ResponseDTO> GetUserById(int id)
		{
			var user = await _userAccountRepository.GetUserAccountById(id);
			return user == null
				? new ResponseDTO { Code = 400, Message = "Người dùng không tồn tại" }
				: new ResponseDTO { Data = _mapper.Map<UserAccountDTO>(user) };
		}

		public async Task<ResponseDTO> GetUserByUsername(string username)
		{
			var user = await _userAccountRepository.GetUserByUsername(username);
			return user == null
				? new ResponseDTO { Code = 400, Message = "Người dùng không tồn tại" }
				: new ResponseDTO { Data = _mapper.Map<UserAccountDTO>(user) };
		}

		public async Task<ResponseDTO> GetUsers(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
		{
			var publishers = _userAccountRepository.GetAllUserAccount(page, pageSize, key, sortBy);
			return new ResponseDTO()
			{
				Data = _mapper.Map<List<UserAccountDTO>>(publishers),
				Total = _userAccountRepository.Total
			};
		}



		public async Task<ResponseDTO> RestoreUser(int id)
		{
			var user = await _userAccountRepository.GetUserAccountById(id);
			if (user == null)
				return new ResponseDTO { Code = 400, Message = "Người dùng không tồn tại" };

			user.IsDeleted = false;
			await _userAccountRepository.UpdateUserAccount(id, user);
			return await _userAccountRepository.IsSaveChanges()
				? new ResponseDTO { Code = 200, Message = "Khôi phục thành công" }
				: new ResponseDTO { Code = 400, Message = "Khôi phục thất bại" };
		}



		public async Task<ResponseDTO> UpdateUser(int id, UserAccountDTO userAccountDTO)
		{
			var user = await _userAccountRepository.GetUserAccountById(id);
			if (user == null)
				return new ResponseDTO { Code = 400, Message = "Người dùng không tồn tại" };

			user.FirstName = userAccountDTO.FirstName;
			user.LastName = userAccountDTO.LastName;
			user.Email = userAccountDTO.Email;
			user.Phone = userAccountDTO.Phone;
			user.Dob = userAccountDTO.Dob;
			user.Avatar = userAccountDTO.Avatar;
			user.RoleId = userAccountDTO.RoleId;
			user.Update = DateTime.Now;

			await _userAccountRepository.UpdateUserAccount(id, user);
			return await _userAccountRepository.IsSaveChanges()
				? new ResponseDTO { Code = 200, Message = "Cập nhật thành công" }
				: new ResponseDTO { Code = 400, Message = "Cập nhật thất bại" };
		}




	}


}
