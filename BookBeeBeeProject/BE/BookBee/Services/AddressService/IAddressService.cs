
using BookBee.DTO.Address;
using BookBee.DTO.Response;

namespace BookBee.Services.AddressService
{
    public interface IAddressService
    {
        Task<ResponseDTO> GetAddresses(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID");
        Task<ResponseDTO> GetAddressByUser(int userId);
        Task<ResponseDTO> GetSelfAddresses();
        Task<ResponseDTO> GetAddressById(int id);
        Task<ResponseDTO> UpdateAddress(int id, AddressDTO addressDTO);
        Task<ResponseDTO> DeleteAddress(int id);
        Task<ResponseDTO> CreateAddress(AddressDTO addressDTO);
		Task<ResponseDTO> SelfCreateAddress(AddressDTO sddressDTO);
    }
}
