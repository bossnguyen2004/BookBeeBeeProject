using BookBee.DTO.Response;
using BookBee.Model;

namespace BookBee.Persistences.Repositories.CategoryRepository
{
    public interface ITagRepository
    {
		Task<List<Tag>> GetAllTags(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID");
		Task<Tag> GetTagById(int id);
		Task<ResponseDTO> CreateTag(Tag tag);
		Task<ResponseDTO> UpdateTag(int id, Tag tag);
		Task<ResponseDTO> DeleteTag(int id);
		Task<int> GetTagsCount();
		Task<bool> IsSaveChanges(); 
		int Total { get; }
	}
}
