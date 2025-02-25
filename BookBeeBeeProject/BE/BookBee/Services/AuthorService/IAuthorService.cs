using BookBee.DTO.Author;
using BookBee.DTO.Response;

namespace BookBee.Services.AuthorService
{
    public interface IAuthorService
    {
		Task<ResponseDTO> GetAuthors(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID");
		Task<ResponseDTO> GetAuthorById(int id);
		Task<ResponseDTO> UpdateAuthor(int id, AuthorDTO authorDTO);
		Task<ResponseDTO> DeleteAuthor(int id);
		Task<ResponseDTO> CreateAuthor(string name);
    }
}
