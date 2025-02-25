using BookBee.DTO.Response;
using BookBee.Model;
using Microsoft.EntityFrameworkCore;

namespace BookBee.Persistences.Repositories.EmployeeRepository
{
	public class EmployeeRepository : IEmployeeRepository
	{
		private readonly DataContext _dataContext;
		public int Total { get; set; }

		public EmployeeRepository(DataContext dataContext)
		{
			_dataContext = dataContext;
		}

		public async Task<ResponseDTO> CreateEmployee(Employee employee)
		{
			try
			{
				await _dataContext.Employees.AddAsync(employee);
				return new ResponseDTO { IsSuccess = true, Code = 200, Message = "Thêm nhân viên thành công" };
			}
			catch (Exception)
			{
				return new ResponseDTO { Code = 500, Message = "Thêm nhân viên thất bại" };
			}
		}

		public async Task<ResponseDTO> DeleteEmployee(int id)
		{
			var employee = await _dataContext.Employees.FindAsync(id);
			if (employee == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy nhân viên" };

			try
			{
				_dataContext.Employees.Remove(employee);
				await _dataContext.SaveChangesAsync();
				return new ResponseDTO { Code = 200, Message = "Xóa nhân viên thành công" };
			}
			catch (Exception)
			{
				return new ResponseDTO { Code = 500, Message = "Xóa nhân viên thất bại" };
			}
		}

		public async Task<ResponseDTO> UpdateEmployee(int id, Employee employee)
		{
			var existingEmployee = await _dataContext.Employees.FindAsync(id);
			if (existingEmployee == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy nhân viên" };

			existingEmployee.LastName = employee.LastName;
			existingEmployee.Gender = employee.Gender;
			existingEmployee.BirthYear = employee.BirthYear;
			existingEmployee.Phone = employee.Phone;
			existingEmployee.Hometown = employee.Hometown;

			return new ResponseDTO { Code = 200, Message = "Cập nhật nhân viên thành công" };
		}

		public async Task<Employee> GetEmployeeById(int id)
		{
			return await _dataContext.Employees.FirstOrDefaultAsync(e => e.Id == id);
		}

		public List<Employee> GetEmployees(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
		{
			var query = _dataContext.Employees.AsQueryable();

			if (!string.IsNullOrEmpty(key))
			{
				query = query.Where(e => e.LastName.ToLower().Contains(key.ToLower()) ||
										 e.Hometown.ToLower().Contains(key.ToLower()));
			}

			switch (sortBy)
			{
				case "NAME":
					query = query.OrderBy(e => e.LastName);
					break;
				default:
					query = query.OrderBy(e => e.Id);
					break;
			}

			Total = query.Count();

			return page == null || pageSize == null
				? query.ToList()
				: query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList();
		}

		public async Task<int> GetEmployeeCount()
		{
			return await _dataContext.Employees.CountAsync();
		}

		public async Task<bool> IsSaveChanges()
		{
			return await _dataContext.SaveChangesAsync() > 0;
		}
	}
}
