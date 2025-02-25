using BookBee.DTO.Response;
using BookBee.Model;

namespace BookBee.Persistences.Repositories.PublisherRepository
{
    public interface IPublisherRepository
	{
		Task<ResponseDTO> CreatePublisher(Publisher publisher);
		Task<ResponseDTO> DeletePublisher(int id);
		Task<ResponseDTO> UpdatePublisher(int id, Publisher publisher);
		Task<Publisher> GetPublisherById(int id);
		List<Publisher> GetPublishers(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID");
		Task<int> GetPublishersCount();
		Task<bool> IsSaveChanges();
		int Total { get; }
	}
}
