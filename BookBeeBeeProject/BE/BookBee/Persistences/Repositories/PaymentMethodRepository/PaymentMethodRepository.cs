using BookBee.DTO.Response;
using BookBee.Model;
using BookBee.Persistences.Repositories.AuthorRepository;
using Microsoft.EntityFrameworkCore;

namespace BookBee.Persistences.Repositories.PaymentMethodRepository
{

	public class PaymentMethodRepository : IPaymentMethodRepository
	{
	    private readonly DataContext _dataContext;
	    public int Total { get; set; }
	    
	    public PaymentMethodRepository(DataContext dataContext)
	    {
	    	_dataContext = dataContext;
	    }
	    
	    public async Task<ResponseDTO> CreatePayment(PaymentMethod payment)
	    {
	    	try
	    	{
	    		await _dataContext.PaymentMethods.AddAsync(payment);
	    		return new ResponseDTO { IsSuccess = true, Code = 200, Message = "Success" };
	    	}
	    	catch (Exception)
	    	{
	    		return new ResponseDTO { Code = 500, Message = "Thêm thất bại" };
	    	}
	    }
	    
	    public async Task<ResponseDTO> DeletePayment(int id)
	    {
	    	var author = await _dataContext.PaymentMethods.FindAsync(id);
	    	if (author == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy" };
	    	try
	    	{
	    		 _dataContext.PaymentMethods.Remove(author);
	    		await _dataContext.SaveChangesAsync();
	    		return new ResponseDTO { Code = 200, Message = "Xóa thành công" };
	    	}
	    	catch (Exception)
	    	{
	    		return new ResponseDTO { Code = 500, Message = "Xóa thất bại" };
	    	}
	    }
	    
	    public async Task<ResponseDTO> UpdatePayment(int id, PaymentMethod payment)
	    {
	    	var existingPaymentMethod = await _dataContext.PaymentMethods.FindAsync(id);
	    	if (existingPaymentMethod == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy" };
	    
	    	existingPaymentMethod.CodePay = payment.CodePay;
			existingPaymentMethod.PaymentName = payment.CodePay;
			existingPaymentMethod.Description = payment.CodePay;
			existingPaymentMethod.Status = payment.Status;
			return new ResponseDTO { Code = 200, Message = "Cập nhật thành công" };
	    }
	    
	    public async Task<PaymentMethod> GetPaymentById(int id)
	    {
	    	return await _dataContext.PaymentMethods.FirstOrDefaultAsync(a => a.Id == id);
	    }
	    
	    public List<PaymentMethod> GetPayments(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
	    {
	    	var query = _dataContext.PaymentMethods.Where(a => !a.IsDeleted).AsQueryable();
	    
	    	if (!string.IsNullOrEmpty(key))
	    	{
	    		query = query.Where(a => a.CodePay.ToLower().Contains(key.ToLower()));
	    	}
	    	switch (sortBy)
	    	{
	    		case "NAME":
	    			query = query.OrderBy(u => u.CodePay);
	    			break;

	    		default:
	    			query = query.OrderBy(u => u.IsDeleted).ThenBy(u => u.Id);
	    			break;
	    	}
	    	Total = query.Count();
	    	if (page == null || pageSize == null || sortBy == null) { return query.ToList(); }
	    	else
	    		return query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList();
	    }
	    
	    public async Task<int> GetPaymentCount()
	    {
	    	return await _dataContext.PaymentMethods.CountAsync(t => !t.IsDeleted);
	    }
	    
	    public async Task<bool> IsSaveChanges()
	    {
	    	return await _dataContext.SaveChangesAsync() > 0;
	    }
	}
}

