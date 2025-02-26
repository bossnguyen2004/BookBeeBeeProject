using AutoMapper;
using BookBee.DTO.Address;
using BookBee.DTO.Response;
using BookBee.DTO.Voucher;
using BookBee.Model;
using BookBee.Persistences.Repositories.AddressRepository;
using BookBee.Persistences.Repositories.UserRepository;
using BookBee.Utilities;

namespace BookBee.Services.AddressService
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IUserAccountRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly UserAccessor _userAccessor;
        public AddressService(IAddressRepository addressRepository, IUserAccountRepository userRepository, IMapper mapper, UserAccessor userAccessor)
        {
            _addressRepository = addressRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _userAccessor = userAccessor;
        }

        public async Task<ResponseDTO> CreateAddress(AddressDTO addressDTO)
        {

			var user = await _userRepository.GetUserAccountById(addressDTO.UserAccountId);
			if (user == null)
				return new ResponseDTO { Code = 400, Message = "User không tồn tại" };

			var address = new Address
			{
				Name = addressDTO.Name,
				Street = addressDTO.Street,
				City = addressDTO.City,
				State = addressDTO.State,
				Phone = addressDTO.Phone,
				UserAccountId = addressDTO.UserAccountId,
				Create = DateTime.Now,
				Update = DateTime.Now,
				IsDeleted = false
			};

            await _addressRepository.CreateAddress(address);
			if (await _addressRepository.IsSaveChanges())
			{
				var createdAddress = await _addressRepository.GetAddressById(address.Id);
				return new ResponseDTO
				{
					Message = "Tạo thành công",
					Data = _mapper.Map<AddressDTO>(createdAddress)
				};
			}
			return new ResponseDTO { Code = 400, Message = "Tạo thất bại" };


		}


        public async Task<ResponseDTO> SelfCreateAddress(AddressDTO sddressDTO)
        {
			var userId = _userAccessor.GetCurrentUserId();
			if (userId == null)
				return new ResponseDTO { Code = 404, Message = "User không tồn tại" };

			if (sddressDTO == null)
				return new ResponseDTO { Code = 400, Message = "Dữ liệu không hợp lệ" };

			var address = _mapper.Map<Address>(sddressDTO);
			address.UserAccountId = (int)userId;
			address.Create = DateTime.Now;

			await _addressRepository.CreateAddress(address);
			bool isSaved = await _addressRepository.IsSaveChanges();

			return new ResponseDTO
			{
				Code = isSaved ? 200 : 400,
				Message = isSaved ? "Tạo mới thành công" : "Tạo thất bại",
				Data = isSaved ? address.Id : null
			};
		}

        public async Task<ResponseDTO> DeleteAddress(int id)
        {
            var address =await _addressRepository.GetAddressById(id);
			if (address == null)
				return new ResponseDTO { Code = 400, Message = "Địa chỉ  không tồn tại" };
			if (address.IsDeleted)
				return new ResponseDTO { Code = 400, Message = "Địa chỉ đã bị xóa trước đó" };
			address.IsDeleted = true;
			await _addressRepository.UpdateAddress(id, address);
			bool isSaved = await _addressRepository.IsSaveChanges();
			return new ResponseDTO { Code = isSaved ? 200 : 400, Message = isSaved ? "Xóa thành công" : "Xóa thất bại", Data = isSaved ? new { AddressId = id } : null };
		}

		public async Task<ResponseDTO> GetAddressById(int id)
        {
			var address = await _addressRepository.GetAddressById(id);

			return address == null || address.IsDeleted
	        ? new ResponseDTO { Code = 400, Message = "Địa chỉ không tồn tại hoặc đã bị xóa" }
	        : new ResponseDTO { Code = 200, Message = "Lấy địa chỉ thành công", Data = _mapper.Map<AddressDTO>(address) };
		}

		public async Task<ResponseDTO> GetAddressByUser(int userId)
        {
			var addresses = await _addressRepository.GetAddressByUser(userId);

			return addresses == null || !addresses.Any()
				? new ResponseDTO { Code = 400, Message = "Người dùng không có địa chỉ nào" }
				: new ResponseDTO { Code = 200, Message = "Lấy danh sách địa chỉ thành công", Data = _mapper.Map<List<AddressDTO>>(addresses) };
		}

        public async Task<ResponseDTO> GetSelfAddresses()
        {
            var userId =  _userAccessor.GetCurrentUserId();
            if (userId != null) return await GetAddressByUser((int)userId);
            return new ResponseDTO()
            {
                Code = 400,
                Message = "User không tồn tại"
            };
        }

        public async Task<ResponseDTO> GetAddresses(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
        {
            var addresses = await _addressRepository.GetAddresses(page, pageSize, key, sortBy);
            return new ResponseDTO()
            {
                Data = _mapper.Map<List<AddressDTO>>(addresses),
                Total = await _addressRepository.GetAddressCount()
            };
        }

		public async Task<ResponseDTO> UpdateAddress(int id, AddressDTO addressDTO)
        {
            var address = await _addressRepository.GetAddressById(id);
			if (address == null)
				return new ResponseDTO { Code = 400, Message = "User không tồn tại" };

			address.Update = DateTime.Now;
            address.Name = addressDTO.Name;
            address.Street = addressDTO.Street;
            address.City = addressDTO.City;
            address.State = addressDTO.State;
            address.Phone = addressDTO.Phone;
            address.UserAccountId = addressDTO.UserAccountId;

			await _addressRepository.UpdateAddress(id, address);
			bool isSaved = await _addressRepository.IsSaveChanges();
			return new ResponseDTO { Code = isSaved ? 200 : 400, Message = isSaved ? "Cập nhật thành công" : "Cập nhật thất bại" };
		}
    }
}
