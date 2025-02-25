using BookBee.DTO.Response;
using BookBee.Model;

namespace BookBee.Persistences.Repositories.SupplierRepository
{
	public interface ISupplierRepository
	{
		Task<ResponseDTO> CreateSupplier(Supplier supplier);
		Task<ResponseDTO> DeleteSupplier(int id);
		Task<ResponseDTO> UpdateSupplier(int id, Supplier supplier);
		Task<Supplier> GetSupplierById(int id);
		List<Supplier> GetSupplier(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID");
		Task<int> GetSupplierCount();
		Task<bool> IsSaveChanges();
		int Total { get; }
	}
}
