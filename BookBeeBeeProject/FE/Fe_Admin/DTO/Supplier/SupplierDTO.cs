using System.ComponentModel.DataAnnotations;

namespace Fe_Admin.DTO.Supplier
{
    public class SupplierDTO
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsDeleted { get; set; } = false;


    }
}
