using BookBee.DTO.Publisher;
using BookBee.Services.PublisherService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookBee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PublisherController : ControllerBase
    {
        private readonly IPublisherService _publisherService;
        public PublisherController(IPublisherService publisherService)
        {
            _publisherService = publisherService;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPublisherById(int id)
        {
            var res =await _publisherService.GetPublisherById(id);
            return StatusCode(res.Code, res);
        }
        [HttpGet]
        public async Task<IActionResult> GetPublishers(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
        {
            var res = await _publisherService.GetPublishers(page, pageSize, key, sortBy);
            return StatusCode(res.Code, res);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdatePublisher(int id, [FromBody] PublisherDTO publisherDTODto)
        {
            var res = await _publisherService.UpdatePublisher(id, publisherDTODto);
            return StatusCode(res.Code, res);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeletePublisher(int id)
        {
            var res = await _publisherService.DeletePublisher(id);
            return StatusCode(res.Code, res);
        }
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreatePublisher([FromBody] PublisherDTO publisherDTODto)
        {
            var res = await _publisherService.CreatePublisher(publisherDTODto.Name);
            return StatusCode(res.Code, res);
        }
    }
}
