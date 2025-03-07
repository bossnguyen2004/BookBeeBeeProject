using AutoMapper;
using BookBee.DTO.Response;
using BookBee.Model;
using BookBee.Persistences.Repositories.CartDetailsRepository;
using BookBee.Persistences.Repositories.CartRepository;
using BookBee.Persistences.Repositories.UserRepository;
using BookBee.Utilities;
using BookStack.DTO.Cart;
using BookStack.DTO.CartBook;
using BookStack.Persistence.Repositories.BookRepository;


namespace BookStack.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IUserAccountRepository _userRepository;
        private readonly ICartDetailsRepository _cartDetailsRepository; 
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly UserAccessor _userAccessor;
        public CartService(ICartRepository cartRepository, IMapper mapper, IUserAccountRepository userRepository,IBookRepository bookRepository, UserAccessor userAccessor, ICartDetailsRepository cartDetailsRepository)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _bookRepository = bookRepository;
            _userAccessor = userAccessor;
            _cartDetailsRepository= cartDetailsRepository;

        }

		public async Task<ResponseDTO> AddToCart(int id,int userId, int bookId, int count)
		{
            var book = await _bookRepository.GetBookById(bookId);
            if (book == null)
                return new ResponseDTO { Code = 400, Message = "Sách không tồn tại" };

            if (book.IsDeleted)
                return new ResponseDTO { Code = 400, Message = "Sách hiện không có sẵn" };

            if (count > book.Count)
                return new ResponseDTO { Code = 400, Message = "Số lượng sách không đủ" };

            var cart = await _cartRepository.GetCartByUser(userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    Id = userId,
                    CartDetails = new List<CartDetail>(),
                    Create = DateTime.Now,
                    Update = DateTime.Now
                };
            }

            var cartDetail = cart.CartDetails.FirstOrDefault(cd => cd.BookId == bookId);

            if (cartDetail == null)
            {
                cart.CartDetails.Add(new CartDetail
                {
                    CartId = cart.Id,
                    BookId = bookId,
                    Quantity = count
                });
            }
            else
            {
                int newQuantity = cartDetail.Quantity + count;
                if (newQuantity > book.Count)
                    return new ResponseDTO { Code = 400, Message = "Sản phẩm không đủ số lượng để thêm" };

                if (newQuantity == 0)
                {
                    cart.CartDetails.Remove(cartDetail);
                }
                else
                {
                    cartDetail.Quantity = newQuantity;
                }
            }

            cart.Update = DateTime.Now;
            await _cartRepository.UpdateCart(cart.Id,cart);

            if (await _cartRepository.IsSaveChanges())
                return new ResponseDTO { Code = 200, Message = "Cập nhật giỏ hàng thành công" };
            else
                return new ResponseDTO { Code = 400, Message = "Cập nhật giỏ hàng thất bại" };

        }
        
        public async Task<ResponseDTO> SelfAddToCart(int id, int bookId, int count)
        {
            var userId =  _userAccessor.GetCurrentUserId();
            if (userId != null) return await AddToCart(id,(int)userId, bookId, count);
            return new ResponseDTO
            {
                Code = 400,
                Message = "User không tồn tại"
            };
        }

        public async Task<ResponseDTO> GetCartByUser(int userId)
        {
			var cart = await _cartRepository.GetCartByUser(userId);
			if (cart == null){return new ResponseDTO{Code = 404,Message = "Giỏ hàng của user này không tồn tại"};}
			var cartDTO = _mapper.Map<CartDTO>(cart);
			return new ResponseDTO{Code = 200,Message = "Lấy giỏ hàng thành công",Data = cartDTO};
		}

        public async Task<ResponseDTO> GetSelfCart()
        {
			var userId = _userAccessor.GetCurrentUserId();
			if (!userId.HasValue)
			{
				return new ResponseDTO
				{
					Code = 400,
					Message = "User không tồn tại"
				};
			}

			return await GetCartByUser(userId.Value);
		}

        public async Task<ResponseDTO> IncreaseQuantity(int cartDetailId)
        {
            return await CongOrTruQuantityCartDetail(cartDetailId, 1);
        }

        public async Task<ResponseDTO> DecreaseQuantity(int cartDetailId)
        {
            return await CongOrTruQuantityCartDetail(cartDetailId, -1);
        }


        public async Task<ResponseDTO> CongOrTruQuantityCartDetail(int idCartDetail, int changeAmount)
        {
            try
            {
                var cartDetailX = await _cartDetailsRepository.GetById(idCartDetail);

                if (cartDetailX == null)
                {
                    return new ResponseDTO
                    {
                        Code = 400,
                        Message = "Giỏ hàng chi tiết không tồn tại"
                    };
                }

                // Cập nhật số lượng sản phẩm
                cartDetailX.Quantity += changeAmount;

                // Gọi phương thức đúng để cập nhật
                var response = await _cartDetailsRepository.UpdateAsync(idCartDetail, cartDetailX);

                if (response.IsSuccess)
                {
                    return new ResponseDTO { Code = 200, Message = $"{(changeAmount > 0 ? "Tăng" : "Giảm")} số lượng sản phẩm thành công" };
                }
                else
                {
                    return new ResponseDTO { Code = 400, Message = "Thất bại" };
                }
            }
            catch (Exception e)
            {
                return new ResponseDTO { Code = 500, Message = "Lỗi server: " + e.Message };
            }
        }



        public async Task<ResponseDTO> UpdateQuantity(int cartDetailId, int quantity)
        {
            try
            {
                var cartDetailX = await _cartDetailsRepository.GetById(cartDetailId);

                if (cartDetailX == null)
                {
                    return new ResponseDTO
                    {
                        Code = 400,
                        Message = "Gio Hang chi tiet khong ton tai"
                    };
                }

                cartDetailX.Quantity = quantity; // Cập nhật số lượng trực tiếp vào đối tượng

                var response = await _cartDetailsRepository.UpdateAsync(cartDetailId, cartDetailX); // Gọi đúng phương thức

                if (response.IsSuccess)
                {
                    return new ResponseDTO { Code = 200, Message = "Cập nhật số lượng sản phẩm thành công" };
                }
                else
                {
                    return new ResponseDTO { Code = 400, Message = "Cập nhật thất bại" };
                }
            }
            catch (Exception e)
            {
                return new ResponseDTO { Code = 500, Message = "Lỗi server: " + e.Message };
            }
        }

        public async Task<ResponseDTO> DeleteCartDetail(int cartDetailId)
        {
            try
            {
                var cartDetailX = await _cartDetailsRepository.GetById(cartDetailId);

                if (cartDetailX == null)
                {
                    return new ResponseDTO
                    {
                        Code = 400,
                        Message = "Gio Hang chi tiet khong ton tai"
                    };
                }

                var checkProductDetailInCart = cartDetailX.Quantity;
                CartDetail cartDetail = new CartDetail
                {
                    Id = cartDetailId,
                };

                var response = await _cartDetailsRepository.DeleteAsync(cartDetailId);
                if (response.IsSuccess)
                {
                    return new ResponseDTO { Code = 200, Message = "Xóa giỏ hàng thành công" };
                }
                else
                {
                    return new ResponseDTO { Code = 400, Message = "Xóa giỏ hàng thất bại" };
                }
            }
            catch (Exception e)
            {
                return new ResponseDTO { Code = 400, Message = "Xoa giỏ hàng thất bại" };
            }
        }


      
    }
}
