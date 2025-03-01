using BookBee.DTO.Response;
using BookBee.DTO.Voucher;


namespace BookBee.Services.VoucherService
{
    public interface IVoucherService
    {
		Task<ResponseDTO> GetVoucher(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID", int? status = null);
		Task<ResponseDTO> GetVoucherById(int id);
		Task<ResponseDTO> UpdateVoucher(int id, VoucherDTO voucherDto);
		Task<ResponseDTO> DeleteVoucher(int id);
		Task<ResponseDTO> CreateVoucher(VoucherDTO voucherDto);
		Task<ResponseDTO> ChangeVoucherStatus(int id, int status);
	}
}
