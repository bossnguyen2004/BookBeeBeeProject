using Microsoft.EntityFrameworkCore;

namespace BookBee.Persistences.Repositories.AddressRepository
{
    public class AddressRepository : IAddressRepository
    {
        private readonly DataContext _dataContext;

        public AddressRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void CreateAddress(Model.Address address)
        {
            _dataContext.Addresses.Add(address);

        }

        public void DeleteAddress(Model.Address address)
        {
            _dataContext.Addresses.Remove(address);
        }

        public Model.Address GetAddressById(int? id = 0)
        {
            if (id != 0)
                return _dataContext.Addresses.Include(a => a.UserAccount).FirstOrDefault(a => a.Id == id && a.IsDeleted == false);
            else return _dataContext.Addresses.Include(a => a.UserAccount).Where(a => a.UserAccount.Id == 2 && a.IsDeleted == false).OrderBy(a => a.Id).LastOrDefault();
        }

        public List<Model.Address> GetAddressByUser(int userId)
        {
            return _dataContext.Addresses.Include(a => a.UserAccount).Where(a => a.UserAccount.Id == userId && a.IsDeleted == false).ToList();
        }

        public int GetAddressCount()
        {
            return _dataContext.Addresses.Count();
        }

        public List<Model.Address> GetAddresses(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
        {
            var query = _dataContext.Addresses.Include(a => a.UserAccount).AsQueryable();

            if (!string.IsNullOrEmpty(key))
            {
                query = query.Where(a => (a.Street + ", " + a.City + ", " + a.Phone + ", " + a.State + ", " + a.Phone).ToLower().Contains(key.ToLower()));
            }

            switch (sortBy)
            {
                case "Street":
                    query = query.OrderBy(u => u.Street);
                    break;
                default:
                    query = query.OrderBy(u => u.IsDeleted).ThenBy(u => u.Id);
                    break;
            }
            if (page == null || pageSize == null || sortBy == null) { return query.ToList(); }
            else
                return query.Where(a => a.IsDeleted == false).Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList();
        }

        public bool IsSaveChanges()
        {
            return _dataContext.SaveChanges() > 0;
        }

        public void UpdateAddress(Model.Address address)
        {
            address.Update = DateTime.Now;
            _dataContext.Addresses.Update(address);

        }
    }
}
