using BookBee.DTO.Response;
using BookBee.DTO.Supplier;
using BookBee.DTO.Tag;

namespace BookBee.Services.SupplierService
{
	public interface ISupplierService
	{
		Task<ResponseDTO> GetSuppliers(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID");
		Task<ResponseDTO> GetSuppliersById(int id);
		Task<ResponseDTO> UpdateSuppliers(int id, SupplierDTO supplierDTO);
		Task<ResponseDTO> DeleteSuppliers(int id);
		Task<ResponseDTO> CreateSuppliers(string name);
	}
}
