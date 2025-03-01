using AutoMapper;
using BookBee.DTO.Response;
using BookBee.Model;
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
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly UserAccessor _userAccessor;
        public CartService(ICartRepository cartRepository, IMapper mapper, IUserAccountRepository userRepository,IBookRepository bookRepository, UserAccessor userAccessor)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _bookRepository = bookRepository;
            _userAccessor = userAccessor;

        }

		public async Task<ResponseDTO> AddToCart(int id,int userId, int bookId, int count)
		{ 
			var book = await _bookRepository.GetBookById(bookId);
			if (book == null)
				return new ResponseDTO { Code = 400, Message = "Book không tồn tại" };

			var cart = await _cartRepository.GetCartByUser(userId);
			if (cart == null)
				return new ResponseDTO { Code = 400, Message = "Cart của user không tồn tại" };

			if (book.IsDeleted)
				return new ResponseDTO { Code = 400, Message = "Sản phẩm này hiện không có sẵn" };

			if (count > book.Count)
				return new ResponseDTO { Code = 400, Message = "Sản phẩm hiện không đủ số lượng để thêm" };

			if (count > book.MaxOrder)
				return new ResponseDTO { Code = 400, Message = "Số lượng vượt quá số lượng tối đa có thể đặt hàng" };

			var cartDetail = cart.CartDetails.FirstOrDefault(cd => cd.BookId == bookId);

			if (cartDetail == null)
			{
				cart.CartDetails.Add(new CartDetail()
				{
					CartId = cart.Id,
					BookId = bookId,
					Quantity = count
				});
			}
			else
			{
				int newQuantity = cartDetail.Quantity + count;

				if (newQuantity > book.Count)return new ResponseDTO { Code = 400, Message = "Sản phẩm hiện không đủ số lượng để thêm" };
				if (newQuantity > book.MaxOrder)return new ResponseDTO { Code = 400, Message = "Số lượng vượt quá số lượng tối đa có thể đặt hàng" };
				if (newQuantity == 0){cart.CartDetails.Remove(cartDetail);}
				else{cartDetail.Quantity = newQuantity;}
			}

			cart.Update = DateTime.Now;

			await _cartRepository.UpdateCart(id, cart);

			if (await _cartRepository.IsSaveChanges()){return new ResponseDTO(){Code = 200,Message = "Cập nhật giỏ hàng thành công"};}
			else{return new ResponseDTO(){Code = 400,Message = "Cập nhật giỏ hàng thất bại"};}

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

        
    }
}
