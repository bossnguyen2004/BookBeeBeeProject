using BookBee.DTO.Response;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookBee.Persistences.Repositories.Image
{
    public interface IImageRepository
    {
        List<Model.Image> GetImage();
        public Task<Model.Image> GetImageById(int? id);
        public Task<ResponseDTO> UpdateImage(int id, Model.Image img);
        public Task<ResponseDTO> DeleteImage(int id);
        public Task<ResponseDTO> CreateImage(Model.Image img);
        int GetImageCount();
        bool IsSaveChanges();
    }
}
