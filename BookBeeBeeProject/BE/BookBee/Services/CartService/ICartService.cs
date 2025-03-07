using BookBee.DTO.Response;
using System.Threading.Tasks;

namespace BookStack.Services.CartService
{
    public interface ICartService
    {
        Task<ResponseDTO> GetCartByUser(int userId);
        Task<ResponseDTO> GetSelfCart();
        Task<ResponseDTO> AddToCart(int id,int  userId, int bookId, int count);
		Task<ResponseDTO> SelfAddToCart(int id,int bookId, int count);
        Task<ResponseDTO> IncreaseQuantity(int cartDetailId);
        Task<ResponseDTO> DecreaseQuantity(int cartDetailId);
        Task<ResponseDTO> UpdateQuantity(int cartDetailId, int quantity);
        Task<ResponseDTO> DeleteCartDetail(int cartDetailId);

    }
}
