using System.ComponentModel.DataAnnotations;

namespace Fe_Admin.DTO.Tag
{
    public class TagDTO
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsDeleted { get; set; } = false;

    }
}
