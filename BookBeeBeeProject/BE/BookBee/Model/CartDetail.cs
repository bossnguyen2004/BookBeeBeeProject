using System.ComponentModel.DataAnnotations;

namespace BookBee.Model
{
    public class CartDetail
    {
        [Key]
        public int Id { get; set; }
        public int Quantity { get; set; } = 0;
        public int CartId { get; set; }
        public virtual Cart Cart { get; set; }
        public int BookId { get; set; }
        public virtual Book Book { get; set; }
        public CartDetail() { }
        public CartDetail(int quantity, int cartId, int bookId)
        {
            Quantity = quantity;
            CartId = cartId;
            BookId = bookId;
        }
    }
}
