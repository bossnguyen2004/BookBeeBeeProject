using AutoMapper;
using BookBee.DTO.Order;
using BookBee.DTO.OrderDetail;
using BookBee.DTO.Response;
using BookBee.DTOs.OrderDetail;
using BookBee.Model;
using BookBee.Persistences;
using BookBee.Persistences.Repositories.AddressRepository;
using BookBee.Persistences.Repositories.CartDetailsRepository;
using BookBee.Persistences.Repositories.CartRepository;
using BookBee.Persistences.Repositories.DetailedPaymentRepository;
using BookBee.Persistences.Repositories.EmployeeRepository;
using BookBee.Persistences.Repositories.OrderDetailRepository;
using BookBee.Persistences.Repositories.OrderVoucherRepository;
using BookBee.Persistences.Repositories.PaymentMethodRepository;
using BookBee.Persistences.Repositories.TaiQuayRepository;
using BookBee.Persistences.Repositories.UserRepository;
using BookBee.Services.MailService;
using BookBee.Utilities;
using BookStack.DTOs.Order;
using BookStack.Persistence.Repositories.BookRepository;
using BookStack.Persistence.Repositories.OrderRepository;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace BookBee.Services.TaiQuayServices
{
    public class TaiQuayServices : ITaiQuayServices
    {

        private readonly DataContext _dataContext;
        private readonly IOrderRepository _orderRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IUserAccountRepository _userRepository;
        private readonly IMailService _mailService;
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;
        private readonly OrderDTO _reponseBill;
        private readonly ResponseDTO _reponse;
        private readonly ICartRepository _cartRepository;
        private readonly UserAccessor _userAccessor;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IOrderVoucherRepository _orderVoucherRepository;
        private readonly ITaiQuayRepository _taiQuayRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IDetailedPaymentRepository _detailedPaymentRepository;
        private readonly ICartDetailsRepository _cartDetailsRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        public TaiQuayServices(IOrderRepository orderRepository, IMapper mapper, IBookRepository bookRepository,
            IUserAccountRepository userRepository, IEmployeeRepository employeeRepository,
            IAddressRepository addressRepository, ICartRepository cartRepository, IOrderVoucherRepository orderVoucherRepository,
            UserAccessor userAccessor, IMailService mailService, ICartDetailsRepository cartDetailsRepository, IDetailedPaymentRepository detailedPaymentRepository,
           IPaymentMethodRepository paymentMethodRepository, ITaiQuayRepository taiQuayRepository, DataContext dataContext, IOrderDetailRepository orderDetailRepository)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _reponseBill = new OrderDTO();
            _reponse= new ResponseDTO();
            _bookRepository = bookRepository;
            _userRepository = userRepository;
            _addressRepository = addressRepository;
            _cartRepository = cartRepository;
            _userAccessor = userAccessor;
            _mailService = mailService;
            _employeeRepository = employeeRepository;
            _orderVoucherRepository = orderVoucherRepository;
            _detailedPaymentRepository = detailedPaymentRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _cartDetailsRepository = cartDetailsRepository;
            _taiQuayRepository=taiQuayRepository;
            _dataContext = dataContext;
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task<ResponseDTO> AddBillDetail(string mahoadon, string codeProductDetail, int? soluong)
        {
            try
            {
                if (soluong == null || soluong == 0)
                {
                    soluong = 1;
                }
                else soluong = soluong.Value;

                var sanPhamChiTietDTO1 = _bookRepository.GetBooks(page: null,pageSize: null,key: null,sortBy: null,tagId: null,voucherId: null,includeDeleted: false,publisherId: null,authorId: null,supplierId: null,status: null).Result.FirstOrDefault();
                var sanPhamChiTietDTO = _bookRepository.GetBooks(1).Result.Where(x => x.CodeBook == codeProductDetail).FirstOrDefault();

                var sanPhamChiTiet = await _bookRepository.GetBooks();

                if (sanPhamChiTietDTO == null)
                {
                    return new ResponseDTO { IsSuccess = false, Code = 405, Message = "Sản Phẩm Không Tồn Tại" };
                }

                else if (soluong <= 0)
                {
                    return new ResponseDTO { IsSuccess = false, Code = 400, Message = "so luong phai lon hon 0" };

                }



                var _getBillByInvoiceCode = _taiQuayRepository.GetBillByInvoiceCode(mahoadon).Result;
                if (_getBillByInvoiceCode != null)
                {
                    var checkProductInBillDetail = _orderDetailRepository.GetAllAsync().Result.FirstOrDefault(x => x.OrderId == _getBillByInvoiceCode.Id && x.BookId == sanPhamChiTietDTO.Id);
                    if (checkProductInBillDetail != null)
                    {
                        checkProductInBillDetail.Quantity += (int)soluong;
                        var update = Update(checkProductInBillDetail);
                        if (update.IsSuccess)
                        {
                            var spct = _bookRepository.GetBooks().Result.Where(x => x.Id == sanPhamChiTietDTO.Id).FirstOrDefault();
                            spct.StockQuantity = sanPhamChiTietDTO.StockQuantity - 1;
                            spct.SoldQuantity = sanPhamChiTietDTO.SoldQuantity + 1;
                            _dataContext.Books.Update(spct);
                            _dataContext.SaveChanges();
                            return new ResponseDTO { Content = update.Content, IsSuccess = true, Code = 200, Message = "Cập sản phẩm vào giỏ hàng thành công" };
                        }
                        else if (sanPhamChiTietDTO.StockQuantity < checkProductInBillDetail.Quantity + soluong)
                        {
                            return new ResponseDTO { IsSuccess = false, Code = 400, Message = "Vượt quá số lượng" };

                        }
                        return new ResponseDTO { IsSuccess = false, Code = 400, Message = "Không thể cập nhật số lượng sản phẩm trong giỏ hàng" };


                    }
                    else
                    {
                        var newBillDetail = new OrderDetail
                        {
                            BookId = sanPhamChiTietDTO.Id,
                            OrderId = _getBillByInvoiceCode.Id,
                            Quantity = 1,
                            Price = (double)sanPhamChiTietDTO.Price,

                        };

                        var spct = _bookRepository.GetBooks().Result.Where(x => x.Id == sanPhamChiTietDTO.Id).FirstOrDefault();
                        spct.StockQuantity = sanPhamChiTietDTO.StockQuantity - 1;
                        spct.SoldQuantity = sanPhamChiTietDTO.SoldQuantity + 1;
                        _dataContext.Books.Update(spct);
                        _dataContext.SaveChanges();
                        var create = Create(newBillDetail);
                        if (create.IsSuccess)
                        {
                            return new ResponseDTO { Content = create.Content, IsSuccess = true, Code = 200, Message = "Thêm sản phẩm vào giỏ hàng thành công" };

                        }
                        return new ResponseDTO { IsSuccess = false, Code = 404, Message = "Không thể thêm sản phẩm vào trong giỏ hàng" };

                    }
                }
                else
                {


                    return new ResponseDTO { IsSuccess = false, Code = 405, Message = "Không tìm thấy hóa đơn" };

                }
            }
            catch (Exception e)
            {
                return new ResponseDTO { IsSuccess = false, Code = 500, Message = e.Message };
            }
        }

        public ResponseDTO CapNhatSoLuongHoaDonChiTietTaiQuay(string maHoaDon, string maSPCT, int soLuong)
        {
            try
            {
                var _getBillByInvoiceCode = _taiQuayRepository.GetBillByInvoiceCode(maHoaDon).Result;
                var sanPhamChiTietDTO = _bookRepository.GetBooks(1).Result.Where(x => x.CodeBook == maSPCT).FirstOrDefault();

                var billDetail = _orderDetailRepository.GetAllAsync().Result.FirstOrDefault(x => x.OrderId == _getBillByInvoiceCode.Id && x.BookId == sanPhamChiTietDTO.Id);

                if (billDetail == null)
                {
                    return new ResponseDTO { IsSuccess = false, Code = 404, Message = "Không tìm thấy giỏ hàng chi tiết" };
                }

                if (soLuong > sanPhamChiTietDTO.StockQuantity + billDetail.Quantity)
                {
                    return new ResponseDTO { IsSuccess = false, Code = 404, Message = "Vượt quá số lượng tồn!" };
                }
                var SLThayDoi = soLuong - billDetail.Quantity;
                billDetail.Quantity = soLuong;
                

                var spct = _bookRepository.GetBooks().Result.Where(x => x.Id == sanPhamChiTietDTO.Id).FirstOrDefault();


                spct.StockQuantity = sanPhamChiTietDTO.StockQuantity - SLThayDoi;
                spct.SoldQuantity = sanPhamChiTietDTO.SoldQuantity + SLThayDoi;

                _dataContext.Books.Update(spct);
                _dataContext.SaveChanges();
                var update = Update(billDetail);
                if (update.IsSuccess)
                {
                    return new ResponseDTO { Content = update.Content, IsSuccess = true, Code = 200, Message = "Thành Công Cập nhật số lượng" };

                }
                else
                {
                    return new ResponseDTO { IsSuccess = false, Code = 400, Message = "Không thành công" };
                }
            }
            catch (Exception e)
            {
                return new ResponseDTO { IsSuccess = false, Code = 500, Message = e.Message };

            }
        }

        public ResponseDTO Update(OrderDetail _getBillByInvoiceCode)
        {
            var getById = _dataContext.OrderDetails.FirstOrDefault(x => x.OrderId == _getBillByInvoiceCode.OrderId
            && x.BookId == _getBillByInvoiceCode.BookId);
            if (getById.Quantity <= 0)
            {
                var delete = _dataContext.OrderDetails.Remove(getById);
                _dataContext.SaveChanges();
                return new ResponseDTO { Message = "xóa do số lượng hdct là 0", Code = 200 };
            }
            else
            {
                var result = _dataContext.OrderDetails.Update(getById);
                _dataContext.SaveChanges();
                if (result.State != 0)
                {
                    var allHDTQ = GetAllHDTaiQuay();
                    var HDTQ = allHDTQ.FirstOrDefault(x => x.Id == result.Entity.OrderId).OrderDetailDTOs;
                    var HDCTTQ = HDTQ.FirstOrDefault(x => x.Id == result.Entity.Id);

                    return new ResponseDTO { Content = HDCTTQ, Message = "OK", Code = 200 };
                }
                return new ResponseDTO { Message = "Fail", Code = 400 };
            }

        }

        public ResponseDTO Create(OrderDetail _getBillByInvoiceCode)
        {

            var result = _dataContext.OrderDetails.Add(_getBillByInvoiceCode);
            _dataContext.SaveChanges();

            if (result.State != 0)
            {
                var allHDTQ = GetAllHDTaiQuay();
                var HDTQ = allHDTQ.FirstOrDefault(x => x.Id == result.Entity.OrderId).OrderDetailDTOs;
                var HDCTTQ = HDTQ.FirstOrDefault(x => x.Id == result.Entity.Id);
                return new ResponseDTO { Content = HDCTTQ, Message = "OK", Code = 200 };
            }
            return new ResponseDTO { Message = "Fail", Code = 400 };
        }

        public async Task<ResponseDTO> CreateBill(RequestBillDTO requestBill)
        {
            try
            {
                var user = _dataContext.UserAccounts.FirstOrDefault(x => x.Username == requestBill.Username);

                var tongTienSanPham = 0;
                var tongTienSanPhamSauGiamGia = 0;
                foreach (var x in requestBill.CartItem)
                {

                    tongTienSanPham = (int)(x.Price * x.Quantity);
                    if (x.Price != x.GiaGoc)
                    {
                        tongTienSanPhamSauGiamGia = (int)(x.GiaGoc * x.Quantity);

                    }
                }
                var tienGiamGiaSanPham = tongTienSanPhamSauGiamGia - tongTienSanPham;
                var listVoucher = _orderVoucherRepository.GetOrderVouchers();
                var voucherX = listVoucher.FirstOrDefault(x => x.VoucherCode == requestBill.CodeVoucher);
                var tienGiamVoucher = 0;
                if (voucherX != null)
                {
                    tienGiamVoucher = (int)Math.Round((double)tongTienSanPham * (double)voucherX.Discount / 100);
                }

                else tienGiamVoucher = 0;
                var bill = new Order
                {
                    OrderCode = requestBill.CodeVoucher != null ? requestBill.MaHoaDon : "Bill" + GenerateRandomString(10),
                    CreatedDate = DateTime.Now,
                    DeliveryStatus = 0,
                    PaymentStatus = 0,
                    CustomerName = user != null ? user.Username : requestBill.FullName,
                    PhoneNumber = requestBill.PhoneNumber,
                    ShippingAddress = requestBill.Address,
                    UserAccountId = user != null ? user.Id : null,
                    TotalAmount = requestBill.Payment,
                    DiscountAmount = tienGiamVoucher + tienGiamGiaSanPham,
                    ShippingFee = requestBill.phiship2,
                    OrderVoucherId = voucherX != null ? voucherX.Id : 0
                };
                if (requestBill.trangthaithanhtoan == 1)
                {
                    bill.PaymentDate = DateTime.Now;
                    bill.PaymentStatus = 1;
                    bill.DeliveryStatus = 1;
                }
                var createHoaDon = await _orderRepository.CreateOrder(bill);
                if (createHoaDon.IsSuccess == true)
                {

                    if (requestBill.Username == null)
                    {
                        foreach (var x in requestBill.CartItem)
                        {
                            var spct = _dataContext.Books.FirstOrDefault(a => a.CodeBook == x.CodeBook);
                            var billDetail = new OrderDetail
                            {
                                Id = 0,
                                BookId = spct.Id,
                                Price = x.Price,
                                Quantity = x.Quantity,
                                OrderId = bill.Id
                            };
                            await _orderDetailRepository.CreateAsync(billDetail);
                        }

                    }
                    else
                    {
                        var cartItem = await _cartDetailsRepository.GetCartDetailByUserName(user.Username);
                        foreach (var cartItemDetail in cartItem)
                        {
                            var billDetail = new OrderDetail
                            {
                                Id = 0,
                                BookId = cartItemDetail.BookId,
                                Price = cartItemDetail.Price,
                                Quantity = cartItemDetail.Quantity,
                                OrderId = bill.Id
                            };

                            await _orderDetailRepository.CreateAsync(billDetail);
                        }
                        var gioHang = _dataContext.CartDetails.Where(x => x.CartId == user.Id);
                        _dataContext.RemoveRange(gioHang); _dataContext.SaveChanges();
                    }
                    var pttt = _dataContext.PaymentMethods.FirstOrDefault(x => x.CodePay == requestBill.MaPTTT);
                    if (pttt == null)
                    {
                        var ptttCreate = new Model.PaymentMethod();
                        ptttCreate.PaymentName = requestBill.MaPTTT;
                        ptttCreate.CodePay = requestBill.MaPTTT;
                        ptttCreate.Description = "";
                        ptttCreate.Status = 1;

                        await _paymentMethodRepository.CreatePayment(ptttCreate);
                    }
                    if (requestBill.trangthaithanhtoan == 0)
                    {
                        DetailedPayment phuongThucTTChiTiet = new DetailedPayment()
                        {
                            Id = 0,
                            OrderId = bill.Id,
                            PaymentId = pttt.Id,
                            Status = 0,
                            Price = 0,

                        }; await _dataContext.DetailedPayments.AddAsync(phuongThucTTChiTiet);
                        _dataContext.SaveChanges();
                    }
                    else
                    {
                        var ptttAfterCreate = _dataContext.PaymentMethods.FirstOrDefault(x => x.CodePay == requestBill.MaPTTT);
                        DetailedPayment phuongThucTTChiTiet = new DetailedPayment()
                        {
                            Id = 0,
                            OrderId = bill.Id,
                            PaymentId = ptttAfterCreate.Id,
                            Status = 1,
                            Price = bill.TotalAmount,

                        }; await _dataContext.DetailedPayments.AddAsync(phuongThucTTChiTiet);
                        _dataContext.SaveChanges();
                    }

                    //return SuccessResponse(bill, $"{bill.InvoiceCode}");
                    return new ResponseDTO
                    {
                        Content = bill,
                        Code = 200,
                        Message = "Đặt hàng thành công"
                    };
                }
                else
                    return new ResponseDTO
                    {
                        Content = bill,
                        Code = 400,
                        Message = "Đặt hàng thất bại"
                    };
            }
            catch (Exception e)
            {
                return new ResponseDTO
                {
                    Content = e.Message,
                    Code = 500,
                    Message = "Lỗi không xác định"
                };
            }
        }

        public string TaoMaHoaDon()
        {

            var countHoaDon = _dataContext.Orders.Count(x => x.OrderCode.StartsWith("HDTQ"));
            string maHoaDon = "HDTQ" + (countHoaDon + 1).ToString();
            return maHoaDon;
        }


        public ResponseDTO TaoHoaDonTaiQuay(HDTaiQuayDTO requestHDTaiQuay)
        {
            try
            {

                int countHoaDonCho = GetAllHDTaiQuay().ToList().Count();
                if (countHoaDonCho == 5)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Code = 400,
                        Message = "Chỉ tạo được tối đa 5 hóa đơn chờ"
                    };
                }

                bool userExists = _dataContext.UserAccounts.Any(u => u.Id == requestHDTaiQuay.IdNguoiDung);
                if (!userExists)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Code = 404,
                        Message = "Người dùng không tồn tại!"
                    };
                }

                int maxId = _dataContext.Orders.Max(o => (int?)o.Id) ?? 0;
                int newId = maxId + 1;

                if (newId > 100)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Code = 400,
                        Message = "Đã vượt quá số lượng ID cho phép"
                    };
                }
               

                Order hdtq = new Order()
                {

                    UserAccountId = requestHDTaiQuay.IdNguoiDung,
                    CreatedDate = DateTime.Now,
                    OrderCode = TaoMaHoaDon(),
                    DeliveryStatus = 5, 
                    PaymentStatus = 0,

                };

              var result = _dataContext.Orders.Add(hdtq);
                _dataContext.SaveChanges();

                if (result.State != 0)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = true,
                        Code = 200,
                        Content = hdtq,
                        Message = "Tạo Hóa Đơn Thành Công"
                    };
                }
                else
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Code = 400,
                        Message = "Tạo Hóa Đơn Thất Bại"
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi tạo hóa đơn: {ex.Message}");

                return new ResponseDTO
                {
                    IsSuccess = false,
                    Code = 500,
                    Message = "Lỗi hệ thống, vui lòng thử lại!",
                    Content = ex.ToString()
                };
            }
        }







        public async Task<ResponseDTO> CongQuantityBillDetail(int idBillDetail)
        {
            return await CongOrTruQuantityBillDetail(idBillDetail, 1);
        }

     

        public Task<ResponseDTO> CreateHoaDonTaiQuay(RequestBillDTO requestBill)
        {
            throw new NotImplementedException();
        }

        public List<HoaDonChoDTO> GetAllHDTaiQuay()
        {
            var query = from bill in _dataContext.Orders
                        where bill.DeliveryStatus == 5 && bill.PaymentStatus == 0
                        join v in _dataContext.OrderVouchers on bill.OrderVoucherId equals v.Id into voucherGroup
                        from voucher in voucherGroup.DefaultIfEmpty()

                        join ptttct in _dataContext.DetailedPayments on bill.Id equals ptttct.OrderId into ptttctGroup
                        from thanhtoanchitiet in ptttctGroup.DefaultIfEmpty()

                        join pttt in _dataContext.PaymentMethods on thanhtoanchitiet.PaymentId equals pttt.Id into ptttGroup
                        from phuongthucthanhtoan in ptttGroup.DefaultIfEmpty()

                        join user in _dataContext.UserAccounts on bill.UserAccountId equals user.Id into userGroup
                        from u in userGroup.DefaultIfEmpty()

                        select new HoaDonChoDTO
                        {
                            Id = bill.Id,
                            OrderCode = bill.OrderCode,
                            SDT = bill.PhoneNumber,
                            TenKhachHang = bill.CustomerName,
                            DeliveryStatus = bill.DeliveryStatus,
                            PaymentStatus = bill.PaymentStatus,
                            NgayTao = bill.CreatedDate,
                            TienGiam = (int)bill.DiscountAmount,
                            GiamGia = voucher != null ? Convert.ToInt32(voucher.Discount) : 0,
                            UserAccountId = bill.UserAccountId,
                            TotalAmount = (int)bill.TotalAmount,
                            Phuongthucthanhtoan = phuongthucthanhtoan.PaymentName,
                            NguoiTao = bill.UserAccountId == null ? "..." : u.LastName

                        };
            var result = query.ToList();
            foreach (var item in result)
            {
                item.OrderDetailDTOs = DanhSachHoaDonChiTietTaiQuay(item.OrderCode);
            }
            return result.ToList();
        }


        public List<OrderDetailDTO> DanhSachHoaDonChiTietTaiQuay(string invoiceCode)
        {
            try
            {
                var billDetails = (
                    from x in _dataContext.Orders.AsNoTracking().Where(a => a.OrderCode == invoiceCode)
                    join y in _dataContext.OrderDetails.AsNoTracking() on x.Id equals y.OrderId
                    join z in _dataContext.Books.AsNoTracking() on y.BookId equals z.Id
                    select new OrderDetailDTO
                    {
                        Id = y.Id,
                        OrderId = x.Id,
                        BookId = y.BookId,
                        CodeBook = z.CodeBook,
                        Quantity = y.Quantity,
                        Price = (double)z.GiaThucTe,
                        PriceBan = (double)y.Price
                    }).ToList();
                return billDetails;
            }
            catch (Exception)
            {
                return null;
            }
        }



        public async Task<ResponseDTO> HuyHoaDonAsync(string maHoaDon, string lyDoHuy)
        {
            var _getBillByInvoiceCodez = _taiQuayRepository.GetBillByInvoiceCode(maHoaDon).Result;
            var billDetail = _orderDetailRepository.GetAllAsync().Result.Where(x => x.OrderId == _getBillByInvoiceCodez.Id).ToList();

            if (billDetail.Count() == 0)
            {
                var hoadon = _taiQuayRepository.GetAll().Find(x => x.OrderCode == maHoaDon);
                hoadon.DeliveryStatus = 4;
                hoadon.CancellationReason = lyDoHuy;
                _dataContext.Orders.Update(hoadon);
                _dataContext.SaveChanges();

                return new ResponseDTO { IsSuccess = true, Code = 200, Message = "Hủy hóa đơn thành công" };
            }
            if (billDetail.Any())
            {
                var hoadon = _taiQuayRepository.GetAll().Find(x => x.OrderCode == maHoaDon);
                hoadon.DeliveryStatus = 4;
                hoadon.CancellationReason = lyDoHuy;

                _dataContext.Orders.Update(hoadon);
                _dataContext.SaveChanges();

                foreach (var item in billDetail)
                {
                    await UpdateSoLuongSanPhamChiTietAynsc(item.BookId.Value, -item.Quantity);
                }
                return new ResponseDTO { IsSuccess = true, Code = 200, Message = "Hủy hóa đơn thành côngz" };
            }
            return new ResponseDTO { IsSuccess = false, Code = 400, Message = "Hủy hóa đơn thất bại" };
        }

        public async Task<ResponseDTO> PGetBillByInvoiceCode(string invoiceCode)
        {
            var billT = await _taiQuayRepository.GetBillByInvoiceCode(invoiceCode);
            if (billT != null)
            {
                var listBillDetail = await _taiQuayRepository.GetBillDetailByInvoiceCode(invoiceCode);
                _reponseBill.Id = billT.Id;
                _reponseBill.OrderCode = billT.OrderCode;
                _reponseBill.PhoneNumber = billT.PhoneNumber;
                _reponseBill.CustomerName = billT.CustomerName;
                _reponseBill.ShippingAddress = billT.ShippingAddress;
                _reponseBill.ShippingFee = billT.ShippingFee;
                _reponseBill.DeliveryStatus = billT.DeliveryStatus;
                _reponseBill.PaymentStatus = billT.PaymentStatus;
                _reponseBill.CreatedDate = billT.CreatedDate;
                _reponseBill.PaymentDate = billT.PaymentDate;
                _reponseBill.CodeVoucher = billT.CodeVoucher;
                _reponseBill.GiamGia = billT.GiamGia;
                _reponseBill.DiscountAmount = billT.DiscountAmount;
                _reponseBill.TotalAmount = billT.TotalAmount;
                _reponseBill.UserAccountId = billT.UserAccountId;
                _reponseBill.OrderVoucherId = billT.OrderVoucherId;
                _reponseBill.BillDetail = listBillDetail;
                _reponseBill.IsPayment = billT.IsPayment;
                _reponseBill.Payment = billT.Payment;
                _reponseBill.Count = listBillDetail.Count();
                _reponseBill.Phuongthucthanhtoan = billT.Phuongthucthanhtoan;
                _reponse.Message = $"Lấy hóa đơn {invoiceCode} thành công.";
                _reponse.Content = _reponseBill;
                return _reponse;
            }
            _reponse.Code = 404;
            _reponse.IsSuccess = false;
            _reponse.Message = $"Không tìm thấy hóa đơn {invoiceCode}.";
            return _reponse;
        }


        public bool ThanhToan(Order _hoaDon)
        {
            try
            {
                _dataContext.Orders.Update(_hoaDon);
                _dataContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<ResponseDTO> TruQuantityBillDetail(int idBillDetail)
        {
            return await CongOrTruQuantityBillDetail(idBillDetail, -1);
        }

        public string XoaSanPhamKhoiHoaDon(string maHoaDon, string maSPCT)
        {
            var _getBillByInvoiceCode = _taiQuayRepository.GetBillByInvoiceCode(maHoaDon).Result;
            var sanPhamChiTietDTO = _bookRepository.GetBooks(1).Result.Where(x => x.CodeBook == maSPCT).FirstOrDefault();
            var hoaDonChiTiet = _orderDetailRepository.GetAllAsync().Result.FirstOrDefault(x => x.OrderId == _getBillByInvoiceCode.Id && x.BookId == sanPhamChiTietDTO.Id);
            
            var soLuongSanPham = hoaDonChiTiet.Quantity;
            _dataContext.OrderDetails.Remove(hoaDonChiTiet);

            // cập nhât lại số lượng sản phẩm chi tiết sau khi hủy hóa đơn
            var spct = _bookRepository.GetBooks().Result.Where(x => x.Id == sanPhamChiTietDTO.Id).FirstOrDefault();

            //var spct = new ChiTietSanPham()
            //{

            spct.StockQuantity = sanPhamChiTietDTO.StockQuantity + soLuongSanPham;
            _dataContext.Books.Update(spct);
            _dataContext.SaveChanges();
            return soLuongSanPham.ToString();
        }



        public async Task<ResponseDTO> CongOrTruQuantityBillDetail(int idCartDetail, int changeAmount)
        {
            try
            {
                var billDetail = await _orderDetailRepository.GetByIdAsync(idCartDetail);

                if (billDetail == null)
                {
                    return new ResponseDTO { IsSuccess = false, Code = 405, Message = "Không tìm thấy hóa đơn chi tiết" };

                }

               
                billDetail.Quantity += changeAmount;

                if (billDetail.Quantity == 0)
                {
                    var result = await _orderDetailRepository.DeleteAsync(billDetail.Id);
                    return new ResponseDTO { IsSuccess = true, Code = 200, Message = $"Số lượng bằng 0, xóa sản phẩm khỏi hóa đơn" };
                }
                if (Update(billDetail).IsSuccess)
                {
                  
                    return new ResponseDTO { IsSuccess = true, Code = 200, Message = $"{(changeAmount > 0 ? "Tăng" : "Giảm")} số lượng sản phẩm thành công" };
                }
                else
                {
                    return new ResponseDTO { IsSuccess = false, Code = 400, Message = $"{(changeAmount > 0 ? "Tăng" : "Giảm")} số lượng sản phẩm thất bại" }; ;
                }
            }
            catch (Exception e)
            {
                return new ResponseDTO { IsSuccess = false, Code = 500, Message = e.Message };
            }
        }

        public async Task UpdateSoLuongSanPhamChiTietAynsc(int id, int soLuong)
        {
            try
            {
                var sanPhamChiTiet = await _dataContext.Books.FindAsync(id);
                sanPhamChiTiet!.StockQuantity = sanPhamChiTiet.StockQuantity - soLuong;
                sanPhamChiTiet!.SoldQuantity += soLuong;
                _dataContext.Books.Update(sanPhamChiTiet);
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder randomString = new StringBuilder();

            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                int index = random.Next(chars.Length);
                randomString.Append(chars[index]);
            }

            return randomString.ToString();
        }
    }
}
