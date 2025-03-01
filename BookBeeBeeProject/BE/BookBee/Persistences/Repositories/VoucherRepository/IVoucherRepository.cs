using BookBee.DTO.Response;
using BookBee.Model;

namespace BookBee.Persistences.Repositories.VoucherRepository
{
    public interface IVoucherRepository
    {
		List<Voucher> GetVouchers(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID", int? status = null);
		Task<Voucher> GetVoucherById(int id);
		Task<ResponseDTO> UpdateVoucher(int id, Voucher voucher);
		Task<ResponseDTO> DeleteVoucher(int id);
		Task<ResponseDTO> CreateVoucher(Voucher voucher);
		Task<int> GetVoucherCount();
		Task<bool> IsSaveChanges();
		int Total { get; }
	}
}
