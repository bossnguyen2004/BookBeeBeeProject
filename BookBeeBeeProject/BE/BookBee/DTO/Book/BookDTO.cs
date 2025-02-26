using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookBee.DTO.Book
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Sold { get; set; }
        public string Description { get; set; }
        public int NumberOfPages { get; set; }
        public DateTime PublishDate { get; set; }
        public string Language { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
        public string? ImageUrl { get; set; }
        public string? Format { get; set; }
        public string? PageSize { get; set; }
        public bool IsDeleted { get; set; }
        public int? PublisherId { get; set; }
        public List<int> TagIds { get; set; } 
        public List<int> VoucherIds { get; set; }
		public List<int> ImageIds { get; set; }
		public int? AuthorId { get; set; }
		public int? SupplierId { get; set; }

	}
}
