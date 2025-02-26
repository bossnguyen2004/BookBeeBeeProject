using BookBee.DTO.Response;
using BookBee.Model;

namespace BookBee.Persistences.Repositories.CartRepository
{
    public interface ICartRepository
    {
        Task<ResponseDTO> UpdateCart(int id,Cart cart);
		Task<ResponseDTO> CreateCart(Cart cart);
		Task<Cart> GetCartById(int id);
		Task<ResponseDTO> ClearCartBook(List<int> ids);
		Task<Cart> GetCartByUser(int userId);
		Task<bool> IsSaveChanges();
    }
}
