using BookBee.DTO.Response;
using BookBee.Model;

namespace BookBee.Persistences.Repositories.EmployeeRepository
{
	public interface IEmployeeRepository
	{
		Task<ResponseDTO> CreateEmployee(Employee employee);
		Task<ResponseDTO> DeleteEmployee(int id);
		Task<ResponseDTO> UpdateEmployee(int id, Employee employee);
		Task<Employee> GetEmployeeById(int id);
		List<Employee> GetEmployees(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID");
		Task<int> GetEmployeeCount();
		Task<bool> IsSaveChanges();
		int Total { get; }
	}
}
