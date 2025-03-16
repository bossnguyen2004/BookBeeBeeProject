using BookStack.Services.CartService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookBee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [HttpGet]
        public async Task<IActionResult> GetCartByUser(int userId, string guestCartId)
        {
            var res = await _cartService.GetCartByUser(userId, guestCartId);
            return StatusCode(res.Code, res);
        }


        [HttpPut]
        public async Task<IActionResult> AddToCart([FromBody] int id,int userId, string guestCartId, int bookId, int count)
        {
            var res =await _cartService.AddToCart(id,userId, guestCartId, bookId, count);
            return StatusCode(res.Code, res);
        }





        //[HttpPut("Self")]
        //public async Task<IActionResult> SelfAddToCart(int id,int bookId, int count)
        //{
        //    var res =await _cartService.SelfAddToCart(id,bookId, guestCartId, count);
        //    return StatusCode(res.Code, res);
        //}


        //[HttpGet("Self")]
        //public async Task<IActionResult> GetSelfCart()
        //{
        //    var res =await _cartService.GetSelfCart();
        //    return StatusCode(res.Code, res);
        //}
    }
}
