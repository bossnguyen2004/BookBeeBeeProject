

using BookBee.DTO.Book;
using BookBee.DTO.Response;
using BookBee.DTO.Voucher;
using BookStack.Services.BookService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace BookBee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService; 
      
        public BookController(IBookService bookService)
        {
            _bookService = bookService; 

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var res =await _bookService.GetBookById(id);
            return StatusCode(res.Code, res);
        }
        [HttpPost("ids")]
        public async Task<IActionResult> GetBookByIds(List<int> ids)
        {
            var res =await _bookService.GetBookByIds(ids);
            return StatusCode(res.Code, res);
        }
        [HttpGet]
        public async Task<IActionResult> GetBooks(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID", int? tagId = 0, int? voucherId = 0, bool includeDeleted = false, int? publisherId = null, int? authorId = null, int? status = null)
        {
            var res =await _bookService.GetBooks(page, pageSize, key, sortBy, tagId,voucherId, includeDeleted, publisherId, authorId,status);
            return StatusCode(res.Code, res);
        }

        [HttpGet("TopOrdered")]
        public async Task<IActionResult> GetTopOrderedBooks(int topCount = 10)
        {
            var res =await _bookService.GetTopOrderedBooks(topCount);
            return StatusCode(res.Code, res);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateBook(int id, BookDTO updateBookDTO)
        {
            var res =await _bookService.UpdateBook(id, updateBookDTO);
            return StatusCode(res.Code, res);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var res =await _bookService.DeleteBook(id);
            return StatusCode(res.Code, res);
        }
        [HttpPost("id")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RestoreBook(int id)
        {
            var res =await _bookService.RestoreBook(id);
            return StatusCode(res.Code, res);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ChangeBookStatus(int id, [FromBody] BookStatusDTO statusDto) // ✅ Không dùng BookDTO nữa!
        {
            if (id <= 0 || statusDto == null)
            {
                return BadRequest(new { code = 400, message = "Dữ liệu không hợp lệ." });
            }

            var result = await _bookService.BookStatus(id, statusDto.Status);
            if (result.Code == 200)
            {
                return Ok(new { code = 200, message = "Cập nhật trạng thái thành công!" });
            }

            return BadRequest(new { code = 400, message = result.Message });
        }

        [HttpGet("inactive")]
        public async Task<IActionResult> GetInactiveBooks(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID", int? tagId = 0, int? voucherId = 0, bool includeDeleted = false, int? publisherId = null, int? authorId = null)
        {
            var response = await _bookService.GetInactiveBooks(page, pageSize, key, sortBy, tagId, voucherId, includeDeleted, publisherId, authorId);
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateBook([FromForm] BookDTO bookRequestDto, IFormFile imageFile)
        {
    
            if (imageFile == null || imageFile.Length == 0)
                return BadRequest(new ResponseDTO { Code = 400, Message = "Vui lòng upload ảnh" });

            var allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
            var allowedMimeTypes = new List<string> { "image/jpeg", "image/png", "image/gif" };

            if (!allowedExtensions.Contains(fileExtension) || !allowedMimeTypes.Contains(imageFile.ContentType.ToLower()))
            {
                return BadRequest(new ResponseDTO { Code = 400, Message = "Chỉ chấp nhận ảnh JPG, JPEG, PNG, GIF" });
            }

            // Lưu ảnh vào wwwroot/images
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Lấy base URL của server
            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            // Cập nhật đường dẫn ảnh đầy đủ
            bookRequestDto.ImageUrl = $"{baseUrl}/images/{fileName}";

            var res = await _bookService.CreateBook(bookRequestDto);
            return StatusCode(res.Code, res);

        }
		private static readonly List<string> AllowedMimeTypes = new List<string>
		{
			  "image/jpeg",
			  "image/png",
			  "image/gif"
		};

	}
}
