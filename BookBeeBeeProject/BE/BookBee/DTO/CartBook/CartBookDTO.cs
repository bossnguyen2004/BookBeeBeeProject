using BookBee.DTO.Book;

namespace BookStack.DTO.CartBook
{
    public class CartBookDTO
    {
        public BookDTO Book { get; set; }
        public int Quantity { get; set; }
    }
}
