using System.ComponentModel.DataAnnotations;

namespace Fe_Admin.DTO.Publisher
{
    public class PublisherDTO
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsDeleted { get; set; } = false;

    }
}
