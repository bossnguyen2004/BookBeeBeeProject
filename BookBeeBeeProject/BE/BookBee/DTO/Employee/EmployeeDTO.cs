using System.ComponentModel.DataAnnotations;

namespace BookBee.DTO.Employee
{
	public class EmployeeDTO
	{
		public int Id { get; set; }
		public string LastName { get; set; }
		public int Gender { get; set; }
		public DateTime BirthYear { get; set; }
		public string Phone { get; set; }
		public string Hometown { get; set; }
		public int? Status { get; set; }
		public bool IsDeleted { get; set; } = false;
		public int UserAccountId { get; set; }
	}
}
