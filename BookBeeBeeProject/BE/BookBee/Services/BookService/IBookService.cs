
using BookBee.DTO.Author;
using BookBee.DTO.Book;
using BookBee.DTO.Response;

namespace BookStack.Services.BookService
{
    public interface IBookService
    {
		Task<ResponseDTO> GetBooks(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID", int? tagId = 0,int? voucherId =0,
            bool includeDeleted = false, int? publisherId = null, int? authorId = null, int? supplier= null, int? status = null);
        Task<ResponseDTO> GetTopOrderedBooks(int topCount = 10);
        Task<ResponseDTO> GetBookById(int id);
        Task<ResponseDTO> GetBookByIds(List<int> ids);
        Task<ResponseDTO> UpdateBook(int id, BookDTO BookDTO);
        Task<ResponseDTO> DeleteBook(int id);
        Task<ResponseDTO> RestoreBook(int id);
        Task<ResponseDTO> GetCart(List<int> bookIds);
		Task<ResponseDTO> CreateBook(BookDTO bookDTO);

		Task<ResponseDTO> BookStatus(int id, int status);
	}
}

