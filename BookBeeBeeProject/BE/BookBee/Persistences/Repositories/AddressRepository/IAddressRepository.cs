namespace BookBee.Persistences.Repositories.AddressRepository
{
    public interface IAddressRepository
    {
        List<Model.Address> GetAddresses(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID");
        List<Model.Address> GetAddressByUser(int userId);
        Model.Address GetAddressById(int? id = 0);
        void UpdateAddress(Model.Address address);
        void DeleteAddress(Model.Address address);
        void CreateAddress(Model.Address address);
        int GetAddressCount();
        bool IsSaveChanges();
    }
}
