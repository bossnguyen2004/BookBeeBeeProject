using BookBee.DTO.Tag;
using BookBee.Services.CategoryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookBee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;
        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTagById(int id)
        {
            var res = await _tagService.GetCategorysById(id);
            return StatusCode(res.Code, res);
        }
        [HttpGet]
        public async Task<IActionResult> GetTags(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
        {
            var res =await _tagService.GetCategorys(page, pageSize, key, sortBy);
            return StatusCode(res.Code, res);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateTag(int id, TagDTO tagRequestDto)
        {
            var res =await _tagService.UpdateCategorys(id, tagRequestDto);
            return StatusCode(res.Code, res);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var res =await _tagService.DeleteCategorys(id);
            return StatusCode(res.Code, res);
        }
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateTag([FromBody] TagDTO tagRequestDto)
        {
           
            var res =await _tagService.CreateCategorys(tagRequestDto.Name);
            return StatusCode(res.Code, res);
        }
    }
}
