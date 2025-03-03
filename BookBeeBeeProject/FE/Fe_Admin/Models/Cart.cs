using System.ComponentModel.DataAnnotations;

namespace Fe_Admin.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public int Status { get; set; }
        public DateTime Create { get; set; } = DateTime.Now;
        public DateTime Update { get; set; } = DateTime.Now;
        public virtual List<CartDetail> CartDetails { get; set; } = new List<CartDetail>();
    }
}
