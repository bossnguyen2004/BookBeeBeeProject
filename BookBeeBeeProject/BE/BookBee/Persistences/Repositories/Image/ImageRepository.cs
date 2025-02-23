
using BookBee.DTO.Response;
using BookBee.Model;
using Microsoft.EntityFrameworkCore;
using SendGrid;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookBee.Persistences.Repositories.Image
{
    public class ImageRepository : IImageRepository
    {
        private readonly DataContext _dataContext;
        public ImageRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<ResponseDTO> CreateImage(Model.Image img)
        {
            try
            {
                await _dataContext.Images.AddAsync(img);
                await _dataContext.SaveChangesAsync();
                return new ResponseDTO
                {
                    IsSuccess = true,
                    Code = 200,
                    Message = "Success"
                };
            }
            catch (Exception)
            {
                return new ResponseDTO
                {
                    Code = 500,
                    Message = "Thêm thất bại"
                };
            }
        }

        public async Task<ResponseDTO> DeleteImage(int id)
        {
            var kq = await _dataContext.Images.FindAsync(id);
            try
            {
                _dataContext.Remove(kq);
                await _dataContext.SaveChangesAsync();
                return new ResponseDTO
                {

                    Code = 200,
                    Message = "Xóa thành công"
                };

            }
            catch (Exception)
            {
                return new ResponseDTO
                {
                    Code = 500,
                    Message = "Xóa thất bại"
                };
            }
        }

        public int GetImageCount()
        {
            return _dataContext.Images.Count();
        }

        public List<Model.Image> GetImage()
        {
            return _dataContext.Images.ToList();
        }

        public async Task<Model.Image> GetImageById(int? id)
        {
            return await _dataContext.Images.FindAsync(id);
        }

        public bool IsSaveChanges()
        {
            return _dataContext.SaveChanges() > 0;
        }

        public async Task<ResponseDTO> UpdateImage(int id, Model.Image img)
        {
            var kq = await _dataContext.Images.FindAsync(id);
            kq.MaAnh = img.MaAnh;
            kq.URL = img.URL;
            await _dataContext.SaveChangesAsync();
            return new ResponseDTO
            {

                Code = 200,
                Message = "cap nhat thanh cong"
            };
        }
    }
}
