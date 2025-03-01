using BookBee.DTO.Book;

namespace BookBee.DTOs.OrderDetail
{
    public class OrderDetailDTO
    {
        public BookDTO Book { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}
