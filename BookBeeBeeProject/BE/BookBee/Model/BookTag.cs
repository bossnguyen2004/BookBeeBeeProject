using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookBee.Model
{
    public class BookTag
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Book")]
        public int? IdBook { get; set; }
        public virtual Book? Book { get; set; }

        [ForeignKey("Tag")]
        public int? IdTag { get; set; }
        public virtual Tag? Tag { get; set; }
    }
}
