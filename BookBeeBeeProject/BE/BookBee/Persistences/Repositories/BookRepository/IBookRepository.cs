﻿using BookBee.DTO.Response;
using BookBee.Model;

namespace BookStack.Persistence.Repositories.BookRepository
{
    public interface IBookRepository
    {
        int Total { get; }

		Task<List<Book>> GetBooks(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID",
						  int? tagId = 0, int? voucherId = 0, bool includeDeleted = false,
						  int? publisherId = null, int? authorId = null, int? supplierId = null,int? status=null);

		Task<List<Book>> GetCart(List<int> bookIds);
		Task<Book> GetBookById(int id);
		Task<List<Book>> GetBookByIds(List<int> ids);
		Task<ResponseDTO> UpdateBook(int id, Book book);
		Task<ResponseDTO> DeleteBook(int id);
		Task<ResponseDTO> CreateBook(Book book);
		Task<List<Book>> GetTopOrderedBooks(int topCount = 10);
		Task<bool>  IsSaveChanges();
		Task<int> GetBookCount();

        Task<List<Book>> GetInactiveBooks(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID",
                          int? tagId = 0, int? voucherId = 0, bool includeDeleted = false,
                          int? publisherId = null, int? authorId = null, int? supplierId = null);


        Task<List<Book>> GetLatestBooks(int count = 10);
        Task<ResponseDTO> RestoreBook(int id);
        Task<List<Book>> GetBooksByPriceRange(double fromPrice, double toPrice);
        Task<bool> IsVoucherValid(int voucherId, int status);

    }
}
