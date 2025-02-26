using AutoMapper;
using BookBee.DTO.Author;
using BookBee.DTO.Publisher;
using BookBee.DTO.Response;
using BookBee.Model;
using BookBee.Persistences.Repositories.AuthorRepository;
using System.Net;
using System.Xml.Linq;

namespace BookBee.Services.AuthorService
{
	public class AuthorService : IAuthorService
	{
		private readonly IAuthorRepository _authorRepository;
		private readonly IMapper _mapper;

		public AuthorService(IAuthorRepository authorRepository, IMapper mapper)
		{
			_authorRepository = authorRepository;
			_mapper = mapper;
		}

		public async Task<ResponseDTO> CreateAuthor(string name)
		{
			var author = new Author { Name = name };
			await _authorRepository.CreateAuthor(author);
			if (await _authorRepository.IsSaveChanges()) return new ResponseDTO() { Message = "Tạo thành công" };
			else return new ResponseDTO() { Code = 400, Message = "Tạo thất bại" };
		}

		public async Task<ResponseDTO> DeleteAuthor(int id)
		{
			var author = await _authorRepository.GetAuthorById(id);
			if (author == null)
				return new ResponseDTO { Code = 400, Message = "Author không tồn tại" };
			if (author.IsDeleted)
				return new ResponseDTO { Code = 400, Message = "Author đã bị xóa trước đó" };
			author.IsDeleted = true;
			await _authorRepository.UpdateAuthor(id,author);
			bool isSaved = await _authorRepository.IsSaveChanges();
			return new ResponseDTO { Code = isSaved ? 200 : 400, Message = isSaved ? "Xóa thành công" : "Xóa thất bại" };
		}

		public async Task<ResponseDTO> GetAuthorById(int id)
		{
			var author = await _authorRepository.GetAuthorById(id);

			return author == null || author.IsDeleted
			? new ResponseDTO { Code = 400, Message = "Author  không tồn tại hoặc đã bị xóa" }
			: new ResponseDTO { Code = 200, Message = "Lấy Author  thành công", Data = _mapper.Map<AuthorDTO>(author) };


		}

		public async Task<ResponseDTO> GetAuthors(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
		{
			var authors =  _authorRepository.GetAuthors(page, pageSize, key, sortBy);
			return new ResponseDTO()
			{
				Data = _mapper.Map<List<AuthorDTO>>(authors),
				Total = _authorRepository.Total
			};
		}

		public async Task<ResponseDTO> UpdateAuthor(int id, AuthorDTO authorDTO)
		{
			var author = await _authorRepository.GetAuthorById(id);
			if (author == null)
				return new ResponseDTO { Code = 400, Message = "Author không tồn tại" };
			author.Update = DateTime.Now;
			author.Name = authorDTO.Name;
		    await	_authorRepository.UpdateAuthor(id,author);
			bool isSaved = await _authorRepository.IsSaveChanges();
			return new ResponseDTO { Code = isSaved ? 200 : 400, Message = isSaved ? "Cập nhật thành công" : "Cập nhật thất bại" };
		}
	}
}
