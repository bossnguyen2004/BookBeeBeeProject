using BookBee.DTO.Response;

namespace BookStack.Services.CartService
{
    public interface ICartService
    {
        Task<ResponseDTO> GetCartByUser(int userId);
        Task<ResponseDTO> GetSelfCart();
        Task<ResponseDTO> AddToCart(int id,int  userId, int bookId, int count);
		Task<ResponseDTO> SelfAddToCart(int id,int bookId, int count);
    }
}
