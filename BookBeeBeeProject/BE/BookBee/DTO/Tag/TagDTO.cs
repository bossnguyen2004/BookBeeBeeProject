namespace BookBee.DTO.Tag
{
    public class TagDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }  
        public string? Image { get; set; }
        public bool IsDeleted { get; set; }
    }
}
