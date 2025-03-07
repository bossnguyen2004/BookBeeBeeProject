using BookBee.DTO.Response;
using BookBee.Model;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;

namespace BookBee.Persistences.Repositories.OrderDetailRepository
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly DataContext _dataContext;
        public OrderDetailRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public int Total { get; set; }

        public async Task<ResponseDTO> CreateAsync(OrderDetail HDCT)
        {
            try
            {
                await _dataContext.OrderDetails.AddAsync(HDCT);
                return new ResponseDTO { IsSuccess = true, Code = 200, Message = "Success" };
            }
            catch (Exception)
            {
                return new ResponseDTO { Code = 500, Message = "Thêm thất bại" };
            }
        }

        public async Task<ResponseDTO> DeleteAsync(int id)
        {
            var author = await _dataContext.OrderDetails.FindAsync(id);
            if (author == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy" };
            try
            {
                _dataContext.OrderDetails.Remove(author);
                await _dataContext.SaveChangesAsync();
                return new ResponseDTO { Code = 200, Message = "Xóa thành công" };
            }
            catch (Exception)
            {
                return new ResponseDTO { Code = 500, Message = "Xóa thất bại" };
            }
        }

        public async Task<List<OrderDetail>> GetAllAsync()
        {
            return await _dataContext.OrderDetails.ToListAsync();
        }


        public async Task<OrderDetail> GetByIdAsync(int id)
        {
            return await _dataContext.OrderDetails.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> IsSaveChanges()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<ResponseDTO> UpdateAsync(int id, OrderDetail HDCT)
        {
            var existingAuthor = await _dataContext.OrderDetails.FindAsync(id);
            if (existingAuthor == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy" };

            existingAuthor.Quantity = HDCT.Quantity;
            existingAuthor.Price = HDCT.Price;
            existingAuthor.OrderId = HDCT.OrderId;
            existingAuthor.BookId = HDCT.BookId;
            return new ResponseDTO { Code = 200, Message = "Cập nhật thành công" };
        }
    }
}
