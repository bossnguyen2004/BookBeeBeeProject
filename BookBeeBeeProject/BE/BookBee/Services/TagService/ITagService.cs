using BookBee.DTO.Response;
using BookBee.DTO.Tag;

namespace BookBee.Services.CategoryService
{
    public interface ITagService
    {
		Task<ResponseDTO> GetCategorys(int? page = 1, int? pageSize = 5, string? key = "", string? sortBy = "ID");
		Task<ResponseDTO> GetCategorysById(int id);
		Task<ResponseDTO> UpdateCategorys(int id, TagDTO tagDTO);
		Task<ResponseDTO> DeleteCategorys(int id);
		Task<ResponseDTO> CreateCategorys(string name);
	}
}



