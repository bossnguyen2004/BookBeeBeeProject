using AutoMapper;
using BookBee.DTO.Author;
using BookBee.DTO.Response;
using BookBee.DTO.Tag;
using BookBee.Model;
using BookBee.Persistences;
using BookBee.Persistences.Repositories.CategoryRepository;

namespace BookBee.Services.CategoryService
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;
        public TagService(ITagRepository tagRepository, IMapper mapper)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        public async Task<ResponseDTO> CreateCategorys(string name)
        {
            var tag = new Tag { Name = name};
			await _tagRepository.CreateTag(tag);
            if (await _tagRepository.IsSaveChanges()) return new ResponseDTO() { Message = "Tạo thành công" };
            else return new ResponseDTO() { Code = 400, Message = "Tạo thất bại" };

        }

		public async Task<ResponseDTO> DeleteCategorys(int id)
        {
            var tag = await _tagRepository.GetTagById(id);
            if (tag == null)
				return new ResponseDTO { Code = 400, Message = "Tag không tồn tại" };
			tag.IsDeleted = true;
			await _tagRepository.UpdateTag(id,tag);

			bool isSaved = await _tagRepository.IsSaveChanges();
			return new ResponseDTO { Code = isSaved ? 200 : 400, Message = isSaved ? "Xóa thành công" : "Xóa thất bại" };
		}

		public async Task<ResponseDTO> GetCategorys(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
        {
            var tags = _tagRepository.GetAllTags(page, pageSize, key, sortBy);
            return new ResponseDTO()
            {
                Data = _mapper.Map<List<TagDTO>>(tags),
                Total = _tagRepository.Total
            };
        }

		public async Task<ResponseDTO> GetCategorysById(int id)
        {
            var tag = await _tagRepository.GetTagById(id);
			return tag == null
				 ? new ResponseDTO { Code = 400, Message = "Tag không tồn tại" }
				 : new ResponseDTO { Data = _mapper.Map<TagDTO>(tag) };
		}

		public async Task<ResponseDTO> UpdateCategorys(int id, TagDTO tagDTO)
        {
            var tag = await _tagRepository.GetTagById(id);
            if (tag == null)
				return new ResponseDTO { Code = 400, Message = "Tag không tồn tại" };
			tag.Update = DateTime.Now;
			tag.Name = tagDTO.Name;
			await _tagRepository.UpdateTag(id,tag);
			bool isSaved = await _tagRepository.IsSaveChanges();
			return new ResponseDTO { Code = isSaved ? 200 : 400, Message = isSaved ? "Cập nhật thành công" : "Cập nhật thất bại" };
		}

	}
}
