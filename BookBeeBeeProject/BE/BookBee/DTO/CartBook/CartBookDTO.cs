using BookBee.DTO.Book;

namespace BookStack.DTO.CartBook
{
    public class CartBookDTO
    {
        public int Id { get; set; }
        public int? CartId { get; set; }
        public int? BookId { get; set; }
        public string? CodeBook { get; set; }
        public string? TitleBook { get; set; }
        public int Quantity { get; set; }
        public int SoldQuantity { get; set; }
        public double Price { get; set; } 
        public double GiaGoc { get; set; }
        public int Status { get; set; }
        public string? Anh { get; set; }

    }
}
