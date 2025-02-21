namespace BookBee.Model
{
    public class BookTag
    {
        public int? IdBook { get; set; }
        public virtual Book Book { get; set; }

        public int? IdTag { get; set; }
        public virtual Tag Tags { get; set; }
    }
}
