using System.ComponentModel.DataAnnotations;

namespace BookBee.Model
{
    public class Image
    {
        [Key]
        public int Id { get; set; }
        public string? MaAnh { get; set; }
        public string? URL { get; set; }
        public DateTime Create { get; set; } = DateTime.Now;
        public DateTime Update { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
        public int BookId { get; set; }
        public virtual Book? Book { get; set; }

        public int TagId { get; set; }
        public virtual Tag? Tag { get; set; }

    }
}
