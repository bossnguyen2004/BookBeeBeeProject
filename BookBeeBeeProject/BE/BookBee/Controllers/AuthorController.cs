using BookBee.DTO.Author;
using BookBee.DTO.Response;
using BookBee.Services.AuthorService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BookBee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }
        [HttpGet("{id}")]
		public async Task<IActionResult> GetAuthorById(int id)
        {
            var res = await _authorService.GetAuthorById(id);
            return StatusCode(res.Code, res);
        }
        [HttpGet]
        public async Task<IActionResult> GetAuthors(int? page = 1, int? pageSize = 5, string? key = "", string? sortBy = "ID")
        {
            var res = await _authorService.GetAuthors(page, pageSize, key, sortBy);
            return StatusCode(res.Code, res);

        }
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] AuthorDTO authorDto)
        {
            var res = await _authorService.UpdateAuthor(id, authorDto);
            return StatusCode(res.Code, res);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var res = await _authorService.DeleteAuthor(id);
            return StatusCode(res.Code, res);
        }
       
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateAuthor([FromBody] AuthorDTO authorDto)
        {
            var res = await _authorService.CreateAuthor(authorDto.Name);
            return StatusCode(res.Code, res);
        }

    }
}
