using System.ComponentModel.DataAnnotations;

namespace BookBee.Model
{
    public class CartDetail
    {
        [Key]
        public int Id { get; set; }
        public int CartId { get; set; }
        public int BookId { get; set; }
        public int Quantity { get; set; } = 0;
        public double Price { get; set; }
        public double GiaNhap { get; set; }
        public int Status { get; set; }
        public virtual Book? Book { get; set; }
        public virtual Cart? Cart { get; set; }

    }
}
