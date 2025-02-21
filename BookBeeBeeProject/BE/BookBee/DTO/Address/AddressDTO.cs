namespace BookBee.DTO.Address
{
    public class AddressDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Phone { get; set; }
        public bool IsDeleted { get; set; }
    }
}
