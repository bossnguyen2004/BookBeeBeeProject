using AutoMapper;
using BookBee.DTO.Author;
using BookBee.DTO.Publisher;
using BookBee.DTO.Response;
using BookBee.DTO.Voucher;
using BookBee.Model;
using BookBee.Persistences.Repositories.PublisherRepository;

namespace BookBee.Services.PublisherService
{
    public class PublisherService : IPublisherService
    {
        private readonly IPublisherRepository _publisherRepository;
        private readonly IMapper _mapper;
        public PublisherService(IPublisherRepository publisherRepository, IMapper mapper)
        {
            _publisherRepository = publisherRepository;
            _mapper = mapper;
        }

        public async Task<ResponseDTO> CreatePublisher(string name)
        {
			var publisher = new Publisher { Name = name };
			await _publisherRepository.CreatePublisher(publisher);
            if (await _publisherRepository.IsSaveChanges()) return new ResponseDTO() { Message = "Tạo thành công" };
            else return new ResponseDTO() { Code = 400, Message = "Tạo thất bại" };
        }

		public async Task<ResponseDTO> DeletePublisher(int id)
        {
            var publisher = await _publisherRepository.GetPublisherById(id);
            if (publisher == null)
				return new ResponseDTO { Code = 400, Message = "Publíher không tồn tại" };
			if (publisher.IsDeleted)
				return new ResponseDTO { Code = 400, Message = "Publíher đã bị xóa trước đó" };
			publisher.IsDeleted = true;
			await _publisherRepository.UpdatePublisher(id,publisher);
			bool isSaved = await _publisherRepository.IsSaveChanges();
			return new ResponseDTO { Code = isSaved ? 200 : 400, Message = isSaved ? "Xóa thành công" : "Xóa thất bại" };
		}


		public async Task<ResponseDTO> GetPublisherById(int id)
        {
            var publisher = await _publisherRepository.GetPublisherById(id);
			return publisher == null || publisher.IsDeleted
            ? new ResponseDTO { Code = 400, Message = "Publisher  không tồn tại hoặc đã bị xóa" }
            : new ResponseDTO { Code = 200, Message = "Lấy Publisher  thành công", Data = _mapper.Map<PublisherDTO>(publisher) };

		}

		public async Task<ResponseDTO> GetPublishers(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
        {
            var publishers = _publisherRepository.GetPublishers(page, pageSize, key, sortBy);
            return new ResponseDTO()
            {
                Data = _mapper.Map<List<PublisherDTO>>(publishers),
                Total = _publisherRepository.Total
            };
        }

		public async Task<ResponseDTO> UpdatePublisher(int id, PublisherDTO publisherDTO)
        {
            var publisher = await _publisherRepository.GetPublisherById(id);
            if (publisher == null)
				return new ResponseDTO { Code = 400, Message = "Publíher không tồn tại" };
			publisher.Update = DateTime.Now;
			publisher.Name = publisherDTO.Name;
			await _publisherRepository.UpdatePublisher(id,publisher);
			bool isSaved = await _publisherRepository.IsSaveChanges();
			return new ResponseDTO { Code = isSaved ? 200 : 400, Message = isSaved ? "Cập nhật thành công" : "Cập nhật thất bại" };
		}

	}
}
