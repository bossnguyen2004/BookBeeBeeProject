using BookStack.DTO.CartBook;

namespace BookStack.DTO.Cart
{
    public class CartDTO
    {
        public List<CartBookDTO> CartBooks { get; set; }
        public CartDTO() { }
    }
}
