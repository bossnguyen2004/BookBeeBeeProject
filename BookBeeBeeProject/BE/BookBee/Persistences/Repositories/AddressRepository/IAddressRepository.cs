using BookBee.DTO.Response;
using BookBee.Model;

namespace BookBee.Persistences.Repositories.AddressRepository
{
    public interface IAddressRepository
    {
		Task<List<Model.Address>> GetAddresses(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID");
        Task<List<Model.Address>> GetAddressByUser(int userId);
        Task< Model.Address> GetAddressById(int id);
        Task<ResponseDTO> UpdateAddress(int id, Model.Address address);
        Task<ResponseDTO> DeleteAddress(int id);
		Task<ResponseDTO> CreateAddress(Model.Address address);
       Task<int> GetAddressCount();
		Task<bool> IsSaveChanges();
		int Total { get; }
	}
}
