using BookBee.DTO.Response;
using BookBee.Model;

namespace BookBee.Persistences.Repositories.AuthorRepository
{
    public interface IAuthorRepository
    {
		Task<ResponseDTO> CreateAuthor(Author author);
		Task<ResponseDTO> DeleteAuthor(int id);
		Task<ResponseDTO> UpdateAuthor(int id, Author author);
		Task<Author> GetAuthorById(int id);
		List<Author> GetAuthors(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID");
		Task<int> GetAuthorCount();
		Task<bool> IsSaveChanges();
		int Total { get; }
	}
}
