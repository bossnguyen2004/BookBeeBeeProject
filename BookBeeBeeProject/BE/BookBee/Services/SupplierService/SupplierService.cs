using AutoMapper;
using BookBee.DTO.Author;
using BookBee.DTO.OrderVoucher;
using BookBee.DTO.Response;
using BookBee.DTO.Supplier;
using BookBee.Model;
using BookBee.Persistences.Repositories.AuthorRepository;
using BookBee.Persistences.Repositories.SupplierRepository;

namespace BookBee.Services.SupplierService
{
	public class SupplierService : ISupplierService
	{
		private readonly ISupplierRepository  _supplierRepository;
		private readonly IMapper _mapper;

		public SupplierService(ISupplierRepository supplierRepository, IMapper mapper)
		{
			_supplierRepository = supplierRepository;
			_mapper = mapper;
		}

		public async Task<ResponseDTO> CreateSuppliers(string name)
		{
			var supplier = new Supplier { Name = name };
			await _supplierRepository.CreateSupplier(supplier);
			if (await _supplierRepository.IsSaveChanges()) return new ResponseDTO() { Message = "Tạo thành công" };
			else return new ResponseDTO() { Code = 400, Message = "Tạo thất bại" };
		}

		public async Task<ResponseDTO> DeleteSuppliers(int id)
		{
			var supplier = await _supplierRepository.GetSupplierById(id);
			if (supplier == null)
				return new ResponseDTO { Code = 400, Message = "Supplier không tồn tại" };
			if (supplier.IsDeleted)
				return new ResponseDTO { Code = 400, Message = "Supplier Mại đã bị xóa trước đó" };
			supplier.IsDeleted = true;
			await _supplierRepository.UpdateSupplier(id, supplier);
			bool isSaved = await _supplierRepository.IsSaveChanges();
			return new ResponseDTO { Code = isSaved ? 200 : 400, Message = isSaved ? "Xóa thành công" : "Xóa thất bại" };
		}

		public async Task<ResponseDTO> GetSuppliers(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
		{
			var supplier = _supplierRepository.GetSupplier(page, pageSize, key, sortBy);
			return new ResponseDTO()
			{
				Data = _mapper.Map<List<SupplierDTO>>(supplier),
				Total = _supplierRepository.Total
			};
		}

		public async Task<ResponseDTO> GetSuppliersById(int id)
		{
			var supplier = await _supplierRepository.GetSupplierById(id);

			return supplier == null || supplier.IsDeleted
			? new ResponseDTO { Code = 400, Message = "Supplier  không tồn tại hoặc đã bị xóa" }
			: new ResponseDTO { Code = 200, Message = "Lấy Supplier  thành công", Data = _mapper.Map<SupplierDTO>(supplier) };

		}

		public async Task<ResponseDTO> UpdateSuppliers(int id, SupplierDTO supplierDTO)
		{
			var supplier = await _supplierRepository.GetSupplierById(id);
			if (supplier == null)
				return new ResponseDTO { Code = 400, Message = "Supplier không tồn tại" };
			supplier.Update = DateTime.Now;
			supplier.Name = supplierDTO.Name;
			await _supplierRepository.UpdateSupplier(id, supplier);
			bool isSaved = await _supplierRepository.IsSaveChanges();
			return new ResponseDTO { Code = isSaved ? 200 : 400, Message = isSaved ? "Cập nhật thành công" : "Cập nhật thất bại" };
		}
	}
}
