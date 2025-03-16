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
using System.Threading.Tasks;

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
            _employeeRepository = employeeRepository;
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


            var address = await _addressRepository.GetAddressById(createOrderDTO.AddressId);
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
                            await _bookRepository.UpdateBook(book.Id, book);
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

                if (await _bookRepository.IsSaveChanges())
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

        public async Task<Order> GetOrDerByCode(string orderCode)
        {
            return await _orderRepository.GetOrderByOrderCode(orderCode);
        }
    }
}
