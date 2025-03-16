using BookBee.DTO.PaymentMethod;
using BookBee.Model;
using BookBee.Persistences.Repositories.PaymentMethodRepository;
using BookBee.Services.PaymentMethodService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookBee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PayController : ControllerBase
    {
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IPaymentMethodRepository _IPaymentMethodRepository;
        public PayController(IPaymentMethodService paymentMethodService, IPaymentMethodRepository paymentMethodRepository)
        {
            _paymentMethodService = paymentMethodService;
            _IPaymentMethodRepository = paymentMethodRepository;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentMethodById(int id)
        {
            var res = await _paymentMethodService.GetPaymentMethodById(id);
            return StatusCode(res.Code, res);
        }
        [HttpGet]
        public async Task<IActionResult> GetPaymentMethods(int? page = 1, int? pageSize = 5, string? key = "", string? sortBy = "ID")
        {
            var res = await _paymentMethodService.GetPaymentMethods(page, pageSize, key, sortBy);
            return StatusCode(res.Code, res);

        }
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdatePaymentMethod(int id, [FromBody] PaymentMethodDTO paymentMethodDTO)
        {
            var res = await _paymentMethodService.UpdatePaymentMethod(id, paymentMethodDTO);
            return StatusCode(res.Code, res);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeletePaymentMethod(int id)
        {
            var res = await _paymentMethodService.DeletePaymentMethod(id);
            return StatusCode(res.Code, res);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreatePaymentMethod([FromBody] PaymentMethodDTO paymentMethodDTO)
        {
            var res = await _paymentMethodService.CreatePaymentMethod(paymentMethodDTO);
            return StatusCode(res.Code, res);
        }

        [HttpGet("PhuongThucThanhToanByName")]
        public async Task<IActionResult> PhuongThucThanhToanByName(string name)
        {
            var allPayment = _paymentMethodService.GetAll();
            var phuongThucThanhToan = _paymentMethodService.GetAll().FirstOrDefault(c => c.PaymentName == name);
            if (phuongThucThanhToan == null)
            {
                PaymentMethod pttt = new PaymentMethod()
                {
                    //Id = allPayment.Count+1,
                    PaymentName = name,
                    CodePay = name,
                    Description = name,
                    Status = 1,
                    //Update =DateTime.Now,
                    //Create =DateTime.Now,
                    IsDeleted = false,
                };
                var result = _IPaymentMethodRepository.CreatePayment2(pttt);
                if (result.IsSuccess == true)
                {
                    var phuongThucThanhToan2 = _paymentMethodService.GetAll().FirstOrDefault(c => c.PaymentName == name);
                    return Ok(pttt.Id);
                }
            }
            return Ok(phuongThucThanhToan.Id);
        }

        [HttpPost("AddPhuongThucThanhToanChiTietTaiQuay")]
        public async Task<bool> AddPhuongThucThanhToanChiTietTaiQuay(int IdHoaDon, int IdThanhToan, double SoTien, int TrangThai)
        {
            var count = _paymentMethodService.GetAll().Count;
            var PhuongThucThanhToanChiTiet = new DetailedPayment()
            {
                //Id = count++,
                OrderId = IdHoaDon,
                PaymentId = IdThanhToan,
                Price = SoTien,
                Status = TrangThai
            };
            var result = _paymentMethodService.CreateToDetailPayment2(PhuongThucThanhToanChiTiet);
            if (result.IsSuccess == true)
            {
                return true;

            }
            return false;
        }

    }
}
