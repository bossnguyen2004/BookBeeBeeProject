using AutoMapper;
using BookBee.DTO.Response;
using BookBee.DTOs.OrderDetail;
using BookBee.Model;
using BookBee.Persistences.Repositories.AddressRepository;
using BookBee.Persistences.Repositories.CartRepository;
using BookBee.Persistences.Repositories.EmployeeRepository;
using BookBee.Persistences.Repositories.OrderVoucherRepository;
using BookBee.Persistences.Repositories.UserRepository;
using BookBee.Services.MailService;
using BookBee.Utilities;
using BookStack.DTO.CartBook;
using BookStack.DTOs.Order;
using BookStack.Persistence.Repositories.BookRepository;
using BookStack.Persistence.Repositories.OrderRepository;
using System.Text;

namespace BookBee.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IUserAccountRepository _userRepository;
        private readonly IMailService _mailService;
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;
        private readonly ICartRepository _cartRepository;
        private readonly UserAccessor _userAccessor;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IOrderVoucherRepository _orderVoucherRepository;

        public OrderService(IOrderRepository orderRepository, IMapper mapper, IBookRepository bookRepository,
            IUserAccountRepository userRepository, IEmployeeRepository employeeRepository,
            IAddressRepository addressRepository, ICartRepository cartRepository, IOrderVoucherRepository orderVoucherRepository,
            UserAccessor userAccessor, IMailService mailService)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _bookRepository = bookRepository;
            _userRepository = userRepository;
            _addressRepository = addressRepository;
            _cartRepository = cartRepository;
            _userAccessor = userAccessor;
            _mailService = mailService;
            _employeeRepository= employeeRepository;
            _orderVoucherRepository = orderVoucherRepository;
        }

        public async Task<ResponseDTO> CreateOrder(OrderDTO createOrderDTO)
        {
            var user = await _userRepository.GetUserAccountById(createOrderDTO.UserAccountId);
            if (user == null) return new ResponseDTO()
            {
                Code = 400,
                Message = "User không tồn tại"
            };

            var shippingMode = _orderVoucherRepository.GetOrderVoucherById(createOrderDTO.OrderVoucherId.Value);
            if (shippingMode == null) return new ResponseDTO()
            {
                Code = 400,
                Message = "Vocuher không tồn tại"
            };

            var nhanvien = _employeeRepository.GetEmployeeById(createOrderDTO.EmployeeId.Value);
            if (nhanvien == null) return new ResponseDTO()
            {
                Code = 400,
                Message = "Nhan Vien không tồn tại"
            };


            var address =await _addressRepository.GetAddressById(createOrderDTO.AddressId);
            if (address == null) return new ResponseDTO()
            {
                Code = 400,
                Message = "Address không tồn tại"
            };

            if (user.Addresses.IndexOf(address) < 0)
            {
                return new ResponseDTO()
                {
                    Code = 400,
                    Message = "Địa chỉ không hợp lệ"
                };
            }

            var order = _mapper.Map<Order>(createOrderDTO);
            for (int i = 0; i < createOrderDTO.BookIds.Count; i++)
            {
                var book = await _bookRepository.GetBookById(createOrderDTO.BookIds[i]);
                if (book != null)
                {
                    if (book.IsDeleted)
                    {
                        return new ResponseDTO()
                        {
                            Code = 400,
                            Message = $"Sách {book.Title} hiện không có sẵn"
                        };
                    }

                    if (book.Count < createOrderDTO.QuantitieCounts[i])
                    {
                        return new ResponseDTO()
                        {
                            Code = 400,
                            Message = $"Không đủ số lướng cho sách {book.Title}"
                        };
                    }
                    order.OrderDetails.Add(new OrderDetail()
                    {
                        BookId = book.Id,
                        Quantity = createOrderDTO.QuantitieCounts[i],
                        Price = book.Price
                    });
                }
            }

           await _orderRepository.CreateOrder(order);

            if (await _orderRepository.IsSaveChanges())
            {
                // Retrieve the OrderId after SaveChanges
                var orderId = order.Id;

                // Update book quantities
                foreach (var orderBook in order.OrderDetails)
                {
                    var book = await _bookRepository.GetBookById(orderBook.Id);
                    if (book != null)
                    {
                        // Ensure the book has enough stock
                        if (book.Count >= orderBook.Quantity)
                        {
                            book.Count -= orderBook.Quantity;
                          await  _bookRepository.UpdateBook(book.Id,book);
                        }
                        else
                        {
                            return new ResponseDTO()
                            {
                                Code = 400,
                                Message = $"Không đủ số lượng cho sách ID {book.Id}"
                            };
                        }
                    }
                }

                if ( await _bookRepository.IsSaveChanges())
                {

                    await _cartRepository.ClearCartBook(order.UserAccountId.Value, order.OrderDetails
                                        .Select(c => c.BookId)
                                        .Where(bookId => bookId.HasValue) 
                                        .Select(bookId => bookId.Value)  
                                        .ToList());



                    return new ResponseDTO()
                    {
                        Message = "Tạo thành công",
                        Data = orderId 
                    };
                }

                return new ResponseDTO()
                {
                    Code = 400,
                    Message = "Cập nhật số lượng sách thất bại"
                };
            }

            return new ResponseDTO()
            {
                Code = 400,
                Message = "Tạo thất bại"
            };
        }

        public Task<ResponseDTO> DeleteOrder(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO> GetOrderById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO> GetOrderByUser(int userId, int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID", int? orderType = null)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO> GetOrders(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID", string? status = "", int? orderType = null)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO> GetSelfOrders(int? page = 1, int? pageSize = 10, string? key = "", string? sortBy = "ID")
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO> UpdateOrder(int id, OrderDTO updateOrderDTO)
        {
            throw new NotImplementedException();
        }

        private string GenerateInvoice(Order order)
        {
            var invoiceBuilder = new StringBuilder();

            invoiceBuilder.AppendLine($"<h2>Hóa đơn cho đơn hàng #{order.Id}</h2>");
            invoiceBuilder.AppendLine("<p><strong>Ngày đặt hàng:</strong> " + order.Create.ToString("dd/MM/yyyy HH:mm") + "</p>");
            invoiceBuilder.AppendLine("<br/>");
            invoiceBuilder.AppendLine("<h3>Thông tin khách hàng:</h3>");
            invoiceBuilder.AppendLine($"<p><strong>Họ và tên:</strong> {order.UserAccount.FirstName} {order.UserAccount.LastName}</p>");
            invoiceBuilder.AppendLine($"<p><strong>Email:</strong> {order.UserAccount.Email}</p>");
            invoiceBuilder.AppendLine($"<p><strong>Số điện thoại:</strong> {order.UserAccount.Phone}</p>");
            invoiceBuilder.AppendLine("<br/>");

            invoiceBuilder.AppendLine("<h3>Địa chỉ giao hàng:</h3>");
            invoiceBuilder.AppendLine($"<p><strong>Tên người nhận:</strong> {order.Address.Name}</p>");
            invoiceBuilder.AppendLine($"<p><strong>Địa chỉ:</strong> {order.Address.Street}, {order.Address.City}, {order.Address.State}</p>");
            invoiceBuilder.AppendLine($"<p><strong>Số điện thoại:</strong> {order.Address.Phone}</p>");
            invoiceBuilder.AppendLine("<br/>");

            invoiceBuilder.AppendLine("<h3>Chi tiết đơn hàng:</h3>");
            invoiceBuilder.AppendLine("<table style='width:100%; border-collapse:collapse;'>");
            invoiceBuilder.AppendLine("<thead><tr><th>Tên sách</th><th>Số lượng</th><th>Đơn giá</th><th>Tổng cộng</th></tr></thead>");
            invoiceBuilder.AppendLine("<tbody>");

            double totalAmount = 0;
            foreach (var orderBook in order.OrderDetails)
            {
                var lineTotal = orderBook.Quantity * orderBook.Price;
                invoiceBuilder.AppendLine($"<tr><td>{orderBook.Book.Title}</td><td>{orderBook.Quantity}</td><td>{orderBook.Price.ToString("N0")} VND</td><td>{lineTotal.ToString("N0")} VND</td></tr>");
                totalAmount += lineTotal;
            }

            invoiceBuilder.AppendLine("</tbody></table>");
            invoiceBuilder.AppendLine($"<h3>Tổng cộng: {totalAmount.ToString("N0")} VND</h3>");
            //note line
            invoiceBuilder.AppendLine("<p><strong>Lưu ý: đơn hàng đã bao gồm VAT nhưng chưa bao gồm phí vận chuyển.</strong> </p>");
            invoiceBuilder.AppendLine("<br/>");
            return invoiceBuilder.ToString();
        }

        private string GenerateInvoiceEmail(string invoiceContent)
        {
            return $$"""
                <!DOCTYPE html>
                <html lang="vi">
                <head>
                    <meta charset="UTF-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <style>
                    body {
                        font-family: Arial, sans-serif;
                        background-color: #f4f4f4;
                        margin: 0;
                        padding: 20px;
                    }
                    .email-container {
                        background-color: #ffffff;
                        max-width: 600px;
                        margin: 0 auto;
                        padding: 20px;
                        border-radius: 8px;
                        box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
                    }
                    .logo {
                       text-align: center;
                       margin-bottom: 10px;
                    }
                    .logo img {
                        width: 120px;
                    }
                    .header {
                        text-align: center;
                        padding: 10px 0;
                        color: #333;
                    }
                    .header img {
                        max-width: 150px;
                        height: auto;
                    }
                    .header h1 {
                        color: #007bff;
                        font-size: 24px;
                    }
                    .content {
                        padding: 20px 0;
                        text-align: left;
                    }
                    .content p {
                        color: #555;
                        line-height: 1.5;
                    }
                    .invoice-content {
                        background-color: #f9f9f9;
                        padding: 15px;
                        border-radius: 5px;
                        border: 1px solid #ddd;
                        overflow-x: auto; /* Allows horizontal scrolling if needed */
                    }
                    table {
                        width: 100%;
                        border-collapse: collapse;
                        margin-bottom: 20px;
                        border: 1px solid #ddd;
                    }
                    th, td {
                        border: 1px solid #ddd;
                        padding: 10px;
                        text-align: left;
                        white-space: nowrap; /* Prevents text from wrapping */
                        overflow: hidden; /* Ensures text overflow is hidden */
                        text-overflow: ellipsis; /* Adds ellipsis for overflowed text */
                    }
                    th {
                        background-color: #555; /* Green background for header */
                        color: white;
                    }
                    .footer {
                        text-align: center;
                        padding: 10px 0;
                        color: #888;
                        font-size: 12px;
                        border-top: 1px solid #dddddd;
                        margin-top: 30px;
                    }
                    /* Responsive design */
                    @media screen and (max-width: 600px) {
                        .invoice-content {
                            padding: 10px;
                        }
                        table {
                            width: 100%;
                            border: none;
                        }
                        th, td {
                            display: block;
                            width: 100%;
                            box-sizing: border-box;
                            white-space: normal; /* Allows text wrapping on small screens */
                        }
                    }
                </style>
                </head>
                <body>
                    <div class="email-container">
                        <div class="logo">
                            <img src="https://i.pinimg.com/736x/36/24/e6/3624e650ec342dd00e8bf2b05ead4062.jpg" alt="BookBee Logo">
                        </div>
                        <div class="header">
                            <h1>Hóa đơn từ BookBee</h1>
                        </div>
                        <div class="content">
                            <p>Chào bạn,</p>
                            <p>Cảm ơn bạn đã đặt hàng từ BookBee. Dưới đây là hóa đơn của bạn:</p>
                            <div class="invoice-content">
                                {{invoiceContent}}
                            </div>
                            <p>Xin cảm ơn bạn đã đặt hàng từ BookBee!</p>
                        </div>
                        <div class="footer">
                            <p>&copy; 2024 BookStack. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>
            """;
        }
    }
}
