using Fe_Admin.DTO.Voucher;
using System.ComponentModel.DataAnnotations;

namespace Fe_Admin.DTO.Book
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
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
        public int PublisherId { get; set; }
        public int TagIds { get; set; }
        public int? VoucherIds { get; set; }
        public int AuthorId { get; set; }
        public int SupplierId { get; set; }
        public int? Status { get; set; }
        public IFormFile ImageFile { get; set; }
        public double GiaNhap { get; set; }
        public double GiaThucTe { get; set; }
        public int? StockQuantity { get; set; } = 0;
        public int? SoldQuantity { get; set; } = 0;
    }
}
