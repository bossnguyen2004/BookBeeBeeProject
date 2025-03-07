using BookBee.DTO.Book;

namespace BookBee.DTOs.OrderDetail
{
    public class OrderDetailDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
        public string? CodeBook{ get; set; }
        public string? TitleBook { get; set; }
        public int? BookId { get; set; }
        public double Price { get; set; }
        public double PriceBan { get; set; }
        public string? Description { get; set; }

    }
}
