using BookBee.DTO.Response;
using BookBee.Utilities;
using BookStack.DTO.User;
using BookStack.Persistence.Repositories.OrderRepository;

namespace BookBee.Services.VNPayService
{
	public class VNPayService : IVNPayService
	{
		private readonly IOrderRepository _orderRepository;
		private readonly UserAccessor _userAccessor;
		private readonly string _vnp_ReturnUrl;
		private readonly string _vnp_Url;
		private readonly string _vnp_TmnCode;
		private readonly string _vnp_HashSecret;

		public VNPayService(IOrderRepository orderRepository, IConfiguration configuration, UserAccessor userAccessor)
        {
			_userAccessor = userAccessor;
			_orderRepository = orderRepository;
			_vnp_ReturnUrl = configuration.GetValue<string>("VnPaySettings:vnp_ReturnUrl");
			_vnp_Url = configuration.GetValue<string>("VnPaySettings:vnp_Url");
			_vnp_TmnCode = configuration.GetValue<string>("VnPaySettings:vnp_TmnCode");
			_vnp_HashSecret = configuration.GetValue<string>("VnPaySettings:vnp_HashSecret");
		}
		public async Task<ResponseDTO> CreateUrlPayment(int orderId, double total)
		{
			var orderP = _orderRepository.GetOrderById(orderId);
			if (orderP == null)
			{
				return new ResponseDTO()
				{
					Code = 400,
					Message = "Booking is not valid"
				};
			}

			// Initialize VNPay library and add request data
			VnPayLibrary vnpay = new VnPayLibrary();

			vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
			vnpay.AddRequestData("vnp_Command", "pay");
			vnpay.AddRequestData("vnp_TmnCode", _vnp_TmnCode);
			vnpay.AddRequestData("vnp_Amount", (total * 100).ToString());
			vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
			vnpay.AddRequestData("vnp_CurrCode", "VND");
			vnpay.AddRequestData("vnp_IpAddr", _userAccessor.GetCurrentUserIpAddress() ?? "192.168.1.7");
			vnpay.AddRequestData("vnp_Locale", "vn");
			vnpay.AddRequestData("vnp_OrderInfo", orderId.ToString());
			vnpay.AddRequestData("vnp_OrderType", "150000");
			vnpay.AddRequestData("vnp_ReturnUrl", _vnp_ReturnUrl);
			vnpay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString());
			vnpay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss"));

			// Build URL for VNPAY
			string urlPayment = vnpay.CreateRequestUrl(_vnp_Url, _vnp_HashSecret);

			return new ResponseDTO
			{
				Code = 200,
				Message = "Success",
				Data = urlPayment
			};
		}

		public async Task<ResponseDTO> ReturnPayment(IQueryCollection vnpayData)
		{
			VnPayLibrary vnpay = new VnPayLibrary();

			foreach (var kvp in vnpayData)
			{
				if (!string.IsNullOrEmpty(kvp.Key) && kvp.Key.StartsWith("vnp_"))
				{
					vnpay.AddResponseData(kvp.Key, kvp.Value);
				}
			}

			long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
			string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
			string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
			string vnp_SecureHash = vnpayData.FirstOrDefault(kvp => kvp.Key == "vnp_SecureHash").Value;
			string terminalID = vnpayData.FirstOrDefault(kvp => kvp.Key == "vnp_TmnCode").Value;
			double total = Convert.ToDouble(vnpay.GetResponseData("vnp_Amount")) / 100;
			string paymentInfo = vnpayData.FirstOrDefault(kvp => kvp.Key == "vnp_OrderInfo").Value;
			string bankCode = vnpayData.FirstOrDefault(kvp => kvp.Key == "vnp_BankCode").Value;
			int orderId = Convert.ToInt32(paymentInfo);

			// Validate the VNPAY signature
			bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _vnp_HashSecret);
			if (checkSignature)
			{
				if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
				{
					var order = await _orderRepository.GetOrderById(orderId); // Thêm await

					if (order == null)
					{
						return new ResponseDTO
						{
							Code = 404,
							Message = "Order not found"
						};
					}

					order.Status = 1; // Đảm bảo Status là kiểu int
					await _orderRepository.UpdateOrder(orderId, order); // Thêm await

					if (await _orderRepository.IsSaveChanges())
					{
						string returnUrl = order.UserAccountId == 1
							? "http://localhost:7273/Admin/Orders/History"
							: "http://localhost:7222/Account/History";

						return new ResponseDTO
						{
							Code = 200,
							Message = "Success",
							Data = returnUrl
						};
					}

					return new ResponseDTO { Code = 400, Message = "Failed" };
				}

				return new ResponseDTO
				{
					Code = 500,
					Message = $"An error occurred during processing. Error Code: {vnp_ResponseCode}"
				};
			}

			return new ResponseDTO
			{
				Code = 500,
				Message = "Invalid signature"
			};
		}
	}
}
