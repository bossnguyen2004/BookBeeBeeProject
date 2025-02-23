using BookBee.Persistences;
using BookBee.Persistences.Repositories.Image;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace BookBee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ImageController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IImageRepository _imageRepository;
        public ImageController(DataContext dataContext, IImageRepository imageRepository, IWebHostEnvironment hostingEnvironment)
        {
            _dataContext = dataContext;
            _imageRepository = imageRepository;
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpGet]
        public IEnumerable<Model.Image> GetAll()
        {

            return _imageRepository.GetImage();

        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateImage(string MaAnh, string Url)
        {
            var obj = new Model.Image();
            obj.MaAnh = MaAnh;
            obj.URL = Url;

            if (MaAnh == null || Url == null)
            {
                return BadRequest("Dữ liệu thêm bị trống");
            }
            try

            {
                await _imageRepository.CreateImage(obj);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateImage(int id, string MaAnh, string Url)
        {
            var obj = await _imageRepository.GetImageById(id);
            obj.MaAnh = MaAnh;
            obj.URL = Url;
            try
            {
                await _imageRepository.UpdateImage(id, obj);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteImage(int id)
        {

            var a = await _imageRepository.DeleteImage(id);
            return Ok(a);
        }


        [HttpPost]
        [Route("uploadManyProductDetailImages")] // Test
        public async Task<IActionResult> UploadImages([FromForm] List<IFormFile> files, [FromForm] int bookId, [FromForm] int tagId)
        {
            var objectFolder = "product_images";
            try
            {
                var uploadedFiles = new List<string>();
                var book = await _dataContext.Books.FindAsync(bookId);
                var tag = await _dataContext.Tags.FindAsync(tagId); 

                if (book != null && tag != null) 
                {
                    int i = 1;
                    foreach (var file in files)
                    {
                        if (file != null && file.Length > 0)
                        {
                            string basePath = _hostingEnvironment.WebRootPath;
                            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                            string filePath = Path.Combine(basePath, objectFolder, fileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            uploadedFiles.Add(fileName);
                            int newId = (_dataContext.Images.Max(i => (int?)i.Id) ?? 0) + 1;

                            var image = new Model.Image
                            {
                                Id = newId,
                                MaAnh = "Image" + i++,
                                URL = $"/{objectFolder}/{fileName}",
                                BookId = bookId,
                                TagId = tagId, 
                                              
                            };

                            await _imageRepository.CreateImage(image);
                        }
                    }
                }
                else
                {
                    return BadRequest("Sách hoặc thẻ không tồn tại");
                }

                return Ok(new { Message = "Tải lên thành công", Files = uploadedFiles });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }
    }
}
