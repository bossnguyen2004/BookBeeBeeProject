using BookBee.DTO.Employee;
using BookBee.DTO.Response;

namespace BookBee.Services.EmployeeService
{
	public interface IEmployeeService
	{
		Task<ResponseDTO> CreateEmployee(EmployeeDTO employeeDTO);
		Task<ResponseDTO> DeleteEmployee(int id);
		Task<ResponseDTO> UpdateEmployee(int id, EmployeeDTO employeeDTO);
		Task<ResponseDTO> GetEmployeeById(int id);
		Task<ResponseDTO> ChangeEmployeeStatus(int id, int status);
		Task<ResponseDTO> GetEmployees(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID");
	}
}
