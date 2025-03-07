using BookBee.DTO.Response;
using BookBee.Model;

namespace BookBee.Persistences.Repositories.DetailedPaymentRepository
{
    public class DetailedPaymentRepository : IDetailedPaymentRepository
    {
        private readonly DataContext _dataContext;
        public DetailedPaymentRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<ResponseDTO> Create(DetailedPayment a)
        {
            try
            {
                await _dataContext.DetailedPayments.AddAsync(a);
                return new ResponseDTO { IsSuccess = true, Code = 200, Message = "Success" };
            }
            catch (Exception)
            {
                return new ResponseDTO { Code = 500, Message = "Thêm thất bại" };
            }
        }

        public async Task<ResponseDTO> Delete(int id)
        {
            var a = await _dataContext.DetailedPayments.FindAsync(id);
            if (a == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy" };
            try
            {
                _dataContext.DetailedPayments.Remove(a);
                await _dataContext.SaveChangesAsync();
                return new ResponseDTO { Code = 200, Message = "Xóa thành công" };
            }
            catch (Exception)
            {
                return new ResponseDTO { Code = 500, Message = "Xóa thất bại" };
            }
        }

        public List<DetailedPayment> GetAll()
        {
            return _dataContext.DetailedPayments.ToList();
        }

        public async Task<DetailedPayment> GetById(int id)
        {
            return await _dataContext.DetailedPayments.FindAsync(id);
        }

        public async Task<ResponseDTO> Update(int id, DetailedPayment a)
        {
            var existingAuthor = await _dataContext.DetailedPayments.FindAsync(id);
            if (existingAuthor == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy" };

            existingAuthor.Price = a.Price;
            existingAuthor.OrderId = a.OrderId;
            existingAuthor.PaymentId = a.PaymentId;
            existingAuthor.Status = a.Status;
            return new ResponseDTO { Code = 200, Message = "Cập nhật thành công" };
        }
    }
}
