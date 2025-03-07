using BookBee.DTO.Response;
using BookBee.Model;

namespace BookBee.Persistences.Repositories.OrderDetailRepository
{
    public interface IOrderDetailRepository
    {
        public Task<ResponseDTO> CreateAsync(OrderDetail HDCT);
        public Task<List<OrderDetail>> GetAllAsync();
        public Task<OrderDetail> GetByIdAsync(int id);
        public Task<ResponseDTO> UpdateAsync(int id, OrderDetail HDCT);
        public Task<ResponseDTO> DeleteAsync(int id);
        Task<bool> IsSaveChanges();
        int Total { get; }
    }
}
