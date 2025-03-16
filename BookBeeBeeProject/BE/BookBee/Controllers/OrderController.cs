using BookBee.Services.OrderService;
using BookStack.DTOs.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PdfSharpCore;
using TheArtOfDev.HtmlRenderer.PdfSharp;


namespace BookBee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IWebHostEnvironment environment;
        public OrderController(IOrderService orderService, IWebHostEnvironment webHostEnvironment)
        {
            _orderService = orderService;
            this.environment = webHostEnvironment;

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var res = await _orderService.GetOrderById(id);
            return StatusCode(res.Code, res);
        }
        [HttpGet]
        public async Task<IActionResult> GetOrders(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID", string? status = "")
        {
            var res = await _orderService.GetOrders(page, pageSize, key, sortBy, status);
            return StatusCode(res.Code, res);
        }
        [HttpGet("History")]
        public async Task<IActionResult> GetHistoryOrders(int userId, int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
        {
            var res = await _orderService.GetOrderByUser(userId, page, pageSize, key, sortBy);
            return StatusCode(res.Code, res);
        }
        [HttpGet("Self")]
        public async Task<IActionResult> GetSelfHistoryOrders(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
        {
            var res = await _orderService.GetSelfOrders(page, pageSize, key, sortBy);
            return StatusCode(res.Code, res);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, OrderDTO updateOrderDTO)
        {
            var res = await _orderService.UpdateOrder(id, updateOrderDTO);
            return StatusCode(res.Code, res);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var res = await _orderService.DeleteOrder(id);
            return StatusCode(res.Code, res);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderDTO createOrderDTO)
        {
            var res = await _orderService.CreateOrder(createOrderDTO);
            return StatusCode(res.Code, res);
        }

        //[HttpPost("Self")]
        //public async Task<IActionResult> SelfCreateOrder(OrderDTO selfCreateOrderDTO)
        //{
        //    var res = await _orderService.SelfCreateOrder(selfCreateOrderDTO);
        //    return StatusCode(res.Code, res);
        //}

        [HttpGet("generatepdf")]
        public async Task<IActionResult> GeneratePDF(string InvoiceNo)
        {
            var document = new PdfSharpCore.Pdf.PdfDocument();
            var tongTienSanPham = 0;

            string[] copies = { "Customer copy", "Comapny Copy" };
            //for (int i = 0; i < copies.Length; i++)
            //{
            var order = await _orderService.GetOrDerByCode(InvoiceNo);
            string htmlcontent = "<div style='width:100%; text-align:center'>";
            htmlcontent += "<h2>Welcome to BOOK BEE STORE</h2>";



            if (order != null)
            {
                htmlcontent += "<h2> Invoice No:" + order.OrderCode + " & Invoice Date:" + order.Create + "</h2>";
                htmlcontent += "<h4> Name :" + order.CustomerName + "</h4>";
                htmlcontent += "<h4> Phone Number : " + order.PhoneNumber + "</h4>";
                htmlcontent += "<div>";
            }



            htmlcontent += "<table style ='width:100%; border: 1px solid #000'>";
            htmlcontent += "<thead style='font-weight:bold'>";
            htmlcontent += "<tr>";
            htmlcontent += "<td style='border:1px solid #000'> Book Code </td>";
            htmlcontent += "<td style='border:1px solid #000'> Description </td>";
            htmlcontent += "<td style='border:1px solid #000'>Qty</td>";
            htmlcontent += "<td style='border:1px solid #000'>Price</td >";
            htmlcontent += "<td style='border:1px solid #000'>Total</td>";
            htmlcontent += "</tr>";
            htmlcontent += "</thead >";

            htmlcontent += "<tbody>";
            if (order.OrderDetails != null && order.OrderDetails.Count > 0)
            {

                order.OrderDetails.ForEach(item =>
                {
                    tongTienSanPham += (int)(item.Price * item.Quantity);
                    htmlcontent += "<tr>";
                    htmlcontent += "<td>" + item.Book.CodeBook + "</td>";
                    htmlcontent += "<td>" + item.Book.Title + "</td>";
                    htmlcontent += "<td>" + item.Quantity + "</td >";
                    htmlcontent += "<td>" + item.Price + "</td>";
                    htmlcontent += "<td> " + (item.Price * item.Quantity) + "</td >";
                    htmlcontent += "</tr>";
                });
            }
            htmlcontent += "</tbody>";

            htmlcontent += "</table>";
            htmlcontent += "</div>";

            htmlcontent += "<div style='text-align:right'>";
            htmlcontent += "<table style='border:1px solid #000;float:right' >";
            htmlcontent += "<tr>";
            htmlcontent += "<td style='border:1px solid #000'> Total Product Price </td>";
            htmlcontent += "<td style='border:1px solid #000'> Product Discounts </td>";
            htmlcontent += "<td style='border:1px solid #000'> Money Payable </td>";

            htmlcontent += "</tr>";
            if (order.OrderDetails != null)
            {
                htmlcontent += "<tr>";
                htmlcontent += "<td style='border: 1px solid #000'> " + order.DiscountAmount + "</td>";
                htmlcontent += "<td style='border: 1px solid #000'> " + tongTienSanPham + "</td>";
                htmlcontent += "<td style='border: 1px solid #000'> " + order.TotalAmount + "</td>";
                htmlcontent += "</tr>";
            }
            htmlcontent += "</table>";
            htmlcontent += "</div>";
            htmlcontent += "<h6> Contact : +12341123 & Email : bookbeestore@gmail.com </h6>";
            htmlcontent += "</div>";

            PdfGenerator.AddPdfPages(document, htmlcontent, PageSize.A4);
            //}
            byte[]? response = null;
            using (MemoryStream ms = new MemoryStream())
            {
                document.Save(ms);
                response = ms.ToArray();
            }
            string Filename = "Invoice_" + InvoiceNo + ".pdf";
            return File(response, "application/pdf", Filename);
        }

        [NonAction]
        public string Getbase64string()
        {
            string filepath = this.environment.WebRootPath + "\\Uploads\\common\\logo.jpeg";
            byte[] imgarray = System.IO.File.ReadAllBytes(filepath);
            string base64 = Convert.ToBase64String(imgarray);
            return base64;
        }


    }
}
