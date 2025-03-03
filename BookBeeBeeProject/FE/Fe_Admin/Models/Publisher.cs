using System.ComponentModel.DataAnnotations;

namespace Fe_Admin.Models
{
    public class Publisher
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public DateTime Create { get; set; } = DateTime.Now;
        public DateTime Update { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
    }
}
