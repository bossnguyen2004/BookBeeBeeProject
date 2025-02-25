using BookBee.DTO.Role;

namespace BookBee.DTO.User
{
    public class UserAccountDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool Gender { get; set; }
        public string Phone { get; set; }
        public DateTime Dob { get; set; }
        public string Avatar { get; set; }
        public string Username { get; set; }
		public string Password { get; set; }
		public bool IsDeleted { get; set; }
		public int? RoleId { get; set; }
		public virtual RoleDTO Role { get; set; }
        public string Token { get; set; }
    }
}
