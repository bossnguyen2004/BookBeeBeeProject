using BookBee.DTO.Response;
using BookBee.Model;

namespace BookBee.Persistences.Repositories.DetailedPaymentRepository
{
    public interface IDetailedPaymentRepository
    {
         List<DetailedPayment> GetAll();
         Task<ResponseDTO> Create(DetailedPayment a);
         Task<ResponseDTO> Update(int id, DetailedPayment a);
         Task<DetailedPayment> GetById(int id);
         Task<ResponseDTO> Delete(int id);
    }
}
