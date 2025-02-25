using AutoMapper;
using BookBee.DTO.Employee;
using BookBee.DTO.Response;
using BookBee.DTO.Voucher;
using BookBee.Model;
using BookBee.Persistences.Repositories.EmployeeRepository;
using BookBee.Persistences.Repositories.UserRepository;
using BookBee.Persistences.Repositories.VoucherRepository;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Reflection;

namespace BookBee.Services.EmployeeService
{
	public class EmployeeService: IEmployeeService
	{
		private readonly IEmployeeRepository _employeeRepository;
		private readonly IUserAccountRepository _userAccountRepository;
		private readonly IMapper _mapper;
		public EmployeeService(IEmployeeRepository employeeRepository, IUserAccountRepository userAccountRepository, IMapper mapper)
		{
			_employeeRepository = employeeRepository;
			_userAccountRepository = userAccountRepository;
			_mapper = mapper;
		}

		public async Task<ResponseDTO> ChangeEmployeeStatus(int id, int status)
		{
			var nhanvien = await _employeeRepository.GetEmployeeById(id);
			if (nhanvien == null)
				return new ResponseDTO() { Code = 400, Message = "Nhân Viên không tồn tại" };

			nhanvien.Status = status;
			await _employeeRepository.UpdateEmployee(id, nhanvien);
			if (await _employeeRepository.IsSaveChanges()) return new ResponseDTO() { Message = "Tạo thành công" };
			else return new ResponseDTO() { Code = 400, Message = "Tạo thất bại" };
		}

		public async Task<ResponseDTO> CreateEmployee(EmployeeDTO employeeDTO)
		{

			var nhanvieen = new Employee
			{
				LastName = employeeDTO.LastName,
				Gender = employeeDTO.Gender,
				Phone = employeeDTO.Phone,
				Hometown = employeeDTO.Hometown,
				Status = employeeDTO.Status ?? 1,
				IsDeleted = false
			};

			await _employeeRepository.CreateEmployee(nhanvieen);
			if (await _employeeRepository.IsSaveChanges()) return new ResponseDTO() { Message = "Tạo thành công" };
			else return new ResponseDTO() { Code = 400, Message = "Tạo thất bại" };
		}



		public async Task<ResponseDTO> DeleteEmployee(int id)
		{
			var voucher = await _employeeRepository.GetEmployeeById(id);
			if (voucher == null)
				return new ResponseDTO { Code = 400, Message = "Nhân Viên không tồn tại" };

			voucher.IsDeleted = true;
			await _employeeRepository.UpdateEmployee(id, voucher);
			bool isSaved = await _employeeRepository.IsSaveChanges();
			return new ResponseDTO { Code = isSaved ? 200 : 400, Message = isSaved ? "Xóa thành công" : "Xóa thất bại" };
		}

		public async Task<ResponseDTO> GetEmployees(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
		{
			var nhanvien = _employeeRepository.GetEmployees(page, pageSize, key, sortBy);
			return new ResponseDTO()
			{
				Data = _mapper.Map<List<EmployeeDTO>>(nhanvien),
				Total = _employeeRepository.Total
			};
		}

		public async Task<ResponseDTO> GetEmployeeById(int id)
		{
			var nhanvien = await _employeeRepository.GetEmployeeById(id);
			return nhanvien == null
				 ? new ResponseDTO { Code = 400, Message = "Nhân Viên không tồn tại" }
				 : new ResponseDTO { Data = _mapper.Map<EmployeeDTO>(nhanvien) };
		}

		public async Task<ResponseDTO> UpdateEmployee(int id, EmployeeDTO employeeDTO)
		{
			var nhanvien = await _employeeRepository.GetEmployeeById(id);
			if (nhanvien == null)
				return new ResponseDTO { Code = 400, Message = "Nhân Viên không tồn tại" };

			nhanvien.LastName = employeeDTO.LastName;
			nhanvien.Gender = employeeDTO.Gender;
			nhanvien.Phone = employeeDTO.Phone;
			nhanvien.Hometown = employeeDTO.Hometown;
			nhanvien.Status = employeeDTO.Status ?? nhanvien.Status;

			await _employeeRepository.UpdateEmployee(id, nhanvien);
			bool isSaved = await _employeeRepository.IsSaveChanges();
			return new ResponseDTO { Code = isSaved ? 200 : 400, Message = isSaved ? "Cập nhật thành công" : "Cập nhật thất bại" };
		}
	}
}
