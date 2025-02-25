using BookBee.DTO.Publisher;
using BookBee.DTO.Response;

namespace BookBee.Services.PublisherService
{
    public interface IPublisherService
    {
		Task<ResponseDTO> GetPublishers(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID");
		Task<ResponseDTO> GetPublisherById(int id);
		Task<ResponseDTO> UpdatePublisher(int id, PublisherDTO publisherDTO);
		Task<ResponseDTO> DeletePublisher(int id);
		Task<ResponseDTO> CreatePublisher(string name);
	}
}
