using AutoMapper;
using BookBee.DTO.Address;
using BookBee.DTO.Author;
using BookBee.DTO.Response;
using BookBee.DTO.Voucher;
using BookBee.Model;
using BookBee.Persistences.Repositories.VoucherRepository;
using System.Net;

namespace BookBee.Services.VoucherService
{
    public class VoucherService : IVoucherService
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IMapper _mapper;
        public VoucherService(IVoucherRepository voucherRepository,IMapper mapper)
        {
            _voucherRepository = voucherRepository;
            _mapper = mapper;
        }

        public async Task<ResponseDTO> ChangeVoucherStatus(int id, int status)
        {
            var voucher = await _voucherRepository.GetVoucherById(id);
            if (voucher == null)
                return new ResponseDTO() { Code = 400, Message = "Voucher không tồn tại" };

             voucher.Status = status;
             await   _voucherRepository.UpdateVoucher(id,voucher);
			if (await _voucherRepository.IsSaveChanges()) return new ResponseDTO() { Message = "Tạo thành công" };
			else return new ResponseDTO() { Code = 400, Message = "Tạo thất bại" };
		}

		public async Task<ResponseDTO> CreateVoucher(VoucherDTO voucherDto)
        {
            var voucher = new Voucher
            {
                VoucherCode = voucherDto.VoucherCode,
                VoucherName = voucherDto.VoucherName,
                StartDate = voucherDto.StartDate,
                EndDate = voucherDto.EndDate,
                DiscountValue = voucherDto.DiscountValue,
                Status = voucherDto.Status ?? 1,
                IsDeleted = false
            };

           await _voucherRepository.CreateVoucher(voucher);
            if (await _voucherRepository.IsSaveChanges()) return new ResponseDTO() { Message = "Tạo thành công" };
            else return new ResponseDTO() { Code = 400, Message = "Tạo thất bại" };
        }

		public async Task<ResponseDTO> DeleteVoucher(int id)
        {
            var voucher = await _voucherRepository.GetVoucherById(id);
            if (voucher == null)
				return new ResponseDTO { Code = 400, Message = "Khuyến Mại không tồn tại" };
			if (voucher.IsDeleted)
				return new ResponseDTO { Code = 400, Message = "Khuyến Mại đã bị xóa trước đó" };

			voucher.IsDeleted = true;
			await _voucherRepository.UpdateVoucher(id,voucher);
			bool isSaved = await _voucherRepository.IsSaveChanges();
			return new ResponseDTO { Code = isSaved ? 200 : 400, Message = isSaved ? "Xóa thành công" : "Xóa thất bại" };
		}

		public async Task<ResponseDTO> GetVoucher(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID", int? status = null)
		{
			if (status != null && status != 0 && status != 1) { return new ResponseDTO { Code = 400, Message = "Trạng thái không hợp lệ. Chỉ chấp nhận 0 (Dừng) hoặc 1 (Hoạt động)." }; }
			var vouchers = _voucherRepository.GetVouchers(page, pageSize, key, sortBy,status);
            return new ResponseDTO()
            {
                Data = _mapper.Map<List<VoucherDTO>>(vouchers),
                Total = _voucherRepository.Total
            };
        }

        public async Task<ResponseDTO> GetVoucherById(int id)
        {
            var voucher = await _voucherRepository.GetVoucherById(id);

			return voucher == null || voucher.IsDeleted
		    ? new ResponseDTO { Code = 400, Message = "Khuyến Mại không tồn tại hoặc đã bị xóa" }
		    : new ResponseDTO { Code = 200, Message = "Lấy Khuyến Mại thành công", Data = _mapper.Map<VoucherDTO>(voucher) };

		}

		public async Task<ResponseDTO> UpdateVoucher(int id, VoucherDTO voucherDto)
        {
            var voucher = await _voucherRepository.GetVoucherById(id);
            if (voucher == null)
				return new ResponseDTO { Code = 400, Message = "Khuyến Mại không tồn tại" };

			voucher.Update = DateTime.Now;
            voucher.VoucherCode = voucherDto.VoucherCode;
            voucher.VoucherName = voucherDto.VoucherName;
            voucher.StartDate = voucherDto.StartDate;
            voucher.EndDate = voucherDto.EndDate;
            voucher.DiscountValue = voucherDto.DiscountValue ?? voucher.DiscountValue; 
            voucher.Status = voucherDto.Status ?? voucher.Status;

			await _voucherRepository.UpdateVoucher(id,voucher);
			bool isSaved = await _voucherRepository.IsSaveChanges();
			return new ResponseDTO { Code = isSaved ? 200 : 400, Message = isSaved ? "Cập nhật thành công" : "Cập nhật thất bại" };
		}

	}
}
