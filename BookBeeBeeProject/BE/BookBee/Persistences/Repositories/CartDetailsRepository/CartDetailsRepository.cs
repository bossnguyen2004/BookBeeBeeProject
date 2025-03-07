using BookBee.DTO.Response;
using BookBee.Model;
using BookStack.DTO.CartBook;
using System.Data.Entity;

namespace BookBee.Persistences.Repositories.CartDetailsRepository
{
    public class CartDetailsRepository : ICartDetailsRepository
    {
        private readonly DataContext _dataContext;
        public CartDetailsRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
       

        public async Task<ResponseDTO> CreateAsync(CartDetail obj)
        {
            try
            {
                await _dataContext.CartDetails.AddAsync(obj);
                return new ResponseDTO { IsSuccess = true, Code = 200, Message = "Success" };
            }
            catch (Exception)
            {
                return new ResponseDTO { Code = 500, Message = "Thêm thất bại" };
            }
        }

        public async Task<ResponseDTO> DeleteAsync(int id)
        {
            var cart = await _dataContext.CartDetails.FindAsync(id);
            if (cart == null) return new ResponseDTO { Code = 404, Message = "Không tìm thấy" };
            try
            {
                _dataContext.CartDetails.Remove(cart);
                await _dataContext.SaveChangesAsync();
                return new ResponseDTO { Code = 200, Message = "Xóa thành công" };
            }
            catch (Exception)
            {
                return new ResponseDTO { Code = 500, Message = "Xóa thất bại" };
            }
        }

        public async Task<IEnumerable<CartDetail>> GetAll()
        {
            return await _dataContext.CartDetails.ToListAsync();
        }

        public async Task<CartDetail> GetById(int id)
        {
            return await _dataContext.CartDetails.FindAsync(id);
        }

        public Task<IEnumerable<CartDetail>> GetCartDetailByUserId(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CartBookDTO>> GetCartDetailByUserName(string username)
        {
            var cartItem = await GetCartItem(username);
            if (cartItem == null)
            {
                return null;
            }
            else
            {
                return cartItem.ToList();

            }
        }

        public async Task<ResponseDTO> GetCartJoinForUser(string username)
        {
            try
            {
                var cartItem = await GetCartItem(username); 

                if (cartItem == null)
                {
                    return new ResponseDTO
                    {
                        Content = null,
                        IsSuccess = false,
                        Code = 404,
                        Message = "Không tìm thấy giỏ hàng",
                    };
                }

                return new ResponseDTO
                {
                    Content = cartItem,
                    IsSuccess = true,
                    Code = 200,
                    Message = "Lấy giỏ hàng thành công",
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Content = null,
                    IsSuccess = false,
                    Code = 500,
                    Message = "Lỗi khi lấy giỏ hàng: " + ex.Message
                };
            }
        }

        public async Task<IEnumerable<CartBookDTO>> GetCartItem(string username)
        {
            try
            {
                var user = await _dataContext.UserAccounts.FirstOrDefaultAsync(x => x.Username == username);

                List<CartBookDTO> cartItem = new List<CartBookDTO>();
                cartItem = (
                           from x in await _dataContext.Carts.ToListAsync()
                           join y in await _dataContext.CartDetails.ToListAsync() on x.Id equals y.CartId
                           join a in await _dataContext.Books.ToListAsync() on y.BookId equals a.Id
                           join taggia in await _dataContext.Authors.ToListAsync() on a.AuthorId equals taggia.Id
                           join nxb in await _dataContext.Publishers.ToListAsync() on a.PublisherId equals nxb.Id
                           join sp in await _dataContext.Suppliers.ToListAsync() on a.SupplierId equals sp.Id

                           select new CartBookDTO
                           {
                               Id = y.Id,
                               CartId = x.Id,
                               Quantity = y.Quantity,
                               Status = y.Status,
                               BookId = a.Id,
                               CodeBook = a.CodeBook,
                               SoldQuantity = (int)a.StockQuantity,
                               TitleBook = a.CodeBook + " | " + a.Title, 
                               Price = (double)a.Price,
                               GiaGoc = (double)a.GiaThucTe,

                           }
                    ).ToList();
                return (IEnumerable<CartBookDTO>)cartItem.Where(x => x.CartId == user.Id);
            }
            catch (Exception)
            {
   
                return null;
            }

        }







        public async Task<double> GetTotalPrice(int userId)
        {
            return await _dataContext.CartDetails
         .Where(cd => cd.Cart.Id == userId)
         .SumAsync(cd => cd.Quantity * cd.Book.Price);
        }

        public async Task<bool> IsProductInCart(int userId, string codeProduct)
        {
            return await _dataContext.CartDetails
        .AnyAsync(cd => cd.Cart.Id == userId && cd.Book.CodeBook == codeProduct);
        }

        public async Task<CartDetail> TimGioHangChiTiet(string username, string codeproduct)
        {
            var user = await _dataContext.UserAccounts.FirstOrDefaultAsync(x => x.Username == username);
            var scpct = await _dataContext.Books.FirstOrDefaultAsync(x => x.CodeBook == codeproduct);
            var search = _dataContext.CartDetails.Where(x => x.CartId == user.Id && x.BookId == scpct.Id).First();
            return search;
        }


        public async Task<ResponseDTO> UpdateQuantity(int id, int quantity)
        {
            var cartDetail = await _dataContext.CartDetails.FindAsync(id);
            if (cartDetail == null)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Code = 404,
                    Message = "Không tìm thấy chi tiết giỏ hàng",
                };
            }

            try
            {
                cartDetail.Quantity = quantity;
                _dataContext.CartDetails.Update(cartDetail);
                await _dataContext.SaveChangesAsync();

                return new ResponseDTO
                {
                    IsSuccess = true,
                    Code = 200,
                    Message = "Cập nhật số lượng thành công",
                    Content = cartDetail
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Code = 500,
                    Message = "Lỗi khi cập nhật số lượng: " + ex.Message
                };
            }
        }

        public async Task<ResponseDTO> UpdateAsync(int id, CartDetail obj)
        {
            var cartDT = await _dataContext.CartDetails.FindAsync(id);
            if (cartDT == null)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Code = 404,
                    Message = "Không tìm thấy chi tiết giỏ hàng",
                };
            }

            try
            {
                cartDT.Quantity = obj.Quantity;
                _dataContext.CartDetails.Update(cartDT);
                await _dataContext.SaveChangesAsync();

                return new ResponseDTO
                {
                    IsSuccess = true,
                    Code = 200,
                    Message = "Cập nhật thành công",
                    Content = cartDT
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Code = 500,
                    Message = "Lỗi khi cập nhật: " + ex.Message
                };
            }
        }
    }
}
