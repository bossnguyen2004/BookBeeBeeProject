namespace BookStack.DTO.Cart
{
    public class CreateCartDTO
    {
        public List<int> BookIds { get; set; }
        public List<int> QuantitieCounts { get; set;}
    }
}
