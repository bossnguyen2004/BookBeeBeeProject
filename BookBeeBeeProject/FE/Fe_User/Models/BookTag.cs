namespace Fe_User.Models
{
    public class BookTag
    {
        public int? IdBook { get; set; }
        public virtual Book? Book { get; set; }

        public int? IdTag { get; set; }
        public virtual Tag? Tag { get; set; }
    }
}
