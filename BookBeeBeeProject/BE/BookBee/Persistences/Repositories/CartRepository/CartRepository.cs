using BookBee.DTO.Response;
using BookBee.Model;
using Microsoft.EntityFrameworkCore;

namespace BookBee.Persistences.Repositories.CartRepository
{
    public class CartRepository : ICartRepository
    {
        private readonly DataContext _dataContext;
        public CartRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }


        public async Task<ResponseDTO> CreateCart(Cart cart)
        {
			try
			{
				await _dataContext.Carts.AddAsync(cart);
				return new ResponseDTO { IsSuccess = true, Code = 200, Message = "Success" };
			}
			catch (Exception)
			{
				return new ResponseDTO { Code = 500, Message = "Thêm thất bại" };
			}
		}

        public async Task<Cart> GetCartById(int id)
        {
			return await _dataContext.Carts.FirstOrDefaultAsync(a => a.Id == id);
		}
        public async Task<Cart> GetCartByUser(int userId)
        {
            var user =await _dataContext.UserAccounts.Include(c => c.Cart).FirstOrDefaultAsync(c => c.Id == userId);

            if (user == null) return null;

            var cart =await _dataContext.Carts.Include(c => c.CartDetails).ThenInclude(c => c.Book).FirstOrDefaultAsync(c => c.Id == user.Cart.Id);
            return cart;
        }

        public async Task<ResponseDTO> ClearCartBook(int userId,List<int> ids)
        {
            try
            {
                var userCart = await _dataContext.Carts
                    .Include(c => c.CartDetails)
                    .FirstOrDefaultAsync(c => c.Id == userId);

                if (userCart == null)
                {
                    return new ResponseDTO { Code = 404, Message = "Không tìm thấy giỏ hàng của người dùng." };
                }

                var itemsToDelete = userCart.CartDetails
                    .Where(cd => ids.Contains(cd.BookId))
                    .ToList();

                if (!itemsToDelete.Any())
                {
                    return new ResponseDTO { Code = 400, Message = "Không có sách nào để xóa khỏi giỏ hàng." };
                }

                _dataContext.CartDetails.RemoveRange(itemsToDelete);
                await _dataContext.SaveChangesAsync();

                return new ResponseDTO { Code = 200, Message = "Xóa sách khỏi giỏ hàng thành công." };
            }
            catch (Exception ex)
            {
                return new ResponseDTO { Code = 500, Message = "Lỗi khi xóa sách khỏi giỏ hàng: " + ex.Message };
            }

        }

        public async Task<bool> IsSaveChanges()
        {
			return await _dataContext.SaveChangesAsync() > 0;
		}

        public async Task<ResponseDTO> UpdateCart(int id, Cart cart)
        {
			var existingCart = await _dataContext.Carts.FindAsync(id);
			if (existingCart == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy" };

			existingCart.Status = cart.Status;
			return new ResponseDTO { Code = 200, Message = "Cập nhật thành công" };

		}
    }
}
