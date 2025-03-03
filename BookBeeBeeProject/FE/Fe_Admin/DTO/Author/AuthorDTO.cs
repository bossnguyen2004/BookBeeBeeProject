using System.ComponentModel.DataAnnotations;

namespace Fe_Admin.DTO.Author
{
    public class AuthorDTO
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

    }
}
