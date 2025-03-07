using BookBee.DTO.Response;
using BookBee.Model;
using BookStack.DTO.CartBook;

namespace BookBee.Persistences.Repositories.CartDetailsRepository
{
    public interface ICartDetailsRepository
    {
        Task<ResponseDTO> CreateAsync(CartDetail obj);
        Task<ResponseDTO> UpdateAsync(int id,CartDetail obj);
        Task<ResponseDTO> DeleteAsync(int id);
        Task<CartDetail> GetById(int id);
        Task<IEnumerable<CartDetail>> GetAll();

        Task<ResponseDTO> GetCartJoinForUser(string username);
        Task<IEnumerable<CartBookDTO>> GetCartDetailByUserName(string username);
        Task<IEnumerable<CartDetail>> GetCartDetailByUserId(Guid userId);
        Task<CartDetail> TimGioHangChiTiet(string username, string codeproduct);
        Task<IEnumerable<CartBookDTO>> GetCartItem(string username);
        Task<ResponseDTO> UpdateQuantity(int id, int quantity);
        Task<double> GetTotalPrice(int userId);
        Task<bool> IsProductInCart(int userId, string codeProduct);
    }
}
