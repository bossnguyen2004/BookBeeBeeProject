using BookBee.DTO.Response;
using BookBee.Persistences.Repositories.UserRepository;
using BookStack.Persistence.Repositories.OrderRepository;

namespace BookBee.Services.StatisticalService
{
	public class StatisticalService : IStatisticalService
	{
		private readonly IUserAccountRepository _userRepository;
		private readonly IOrderRepository _orderRepository;
		public StatisticalService(IUserAccountRepository userRepository, IOrderRepository orderRepository)
		{
			_userRepository = userRepository;
			_orderRepository = orderRepository;
		}
		public async Task<object> GetStatisticalUser()
		{
			var users = await _userRepository.GetAllUserAccount(1, 10000, "", "ID");

			int[] arrUser = new int[12];
			DateTime today = DateTime.Today;
			int year = today.Year;
			int month = today.Month;

			int countUser = 0;
			foreach (var user in users.Where(u => u.RoleId == 2 && u.Create.Year == year))
			{
				arrUser[user.Create.Month - 1]++;
				countUser++;
			}

			double diff = (month == 1 || arrUser[month - 2] == 0) ? 0
						 : ((double)(arrUser[month - 1] - arrUser[month - 2]) / arrUser[month - 2]) * 100;

			return new { Total = countUser, Statistical = arrUser, Diff = $"{diff:F2}" };
		}

		public async Task<object> GetStatisticalOrder()
		{
			var orders = await _orderRepository.GetOrders(1, 10000, "", "ID");

			int[] arrOrder = new int[12];
			DateTime today = DateTime.Today;
			int year = today.Year;
			int month = today.Month;

			int countOrder = 0;
			foreach (var order in orders.Where(o => o.Create.Year == year))
			{
				arrOrder[order.Create.Month - 1]++;
				countOrder++;
			}

			double diff = (month == 1 || arrOrder[month - 2] == 0) ? 0
						 : ((double)(arrOrder[month - 1] - arrOrder[month - 2]) / arrOrder[month - 2]) * 100;

			return new { Total = countOrder, Statistical = arrOrder, Diff = $"{diff:F2}" };
		}

		public async Task<object> GetStatisticalRevenue()
		{
			var orders = await _orderRepository.GetOrders(1, 10000, "", "ID");

			double[] arrRevenue = new double[12];
			DateTime today = DateTime.Today;
			int year = today.Year;
			int month = today.Month;

			int countOrder = 0;
			foreach (var order in orders.Where(o => o.Create.Year == year && o.Status == 1))
			{
				arrRevenue[order.Create.Month - 1] += order.OrderDetails.Sum(od => od.Price * od.Quantity);
				countOrder++;
			}

			double diff = (month == 1 || arrRevenue[month - 2] == 0) ? 0
						 : ((arrRevenue[month - 1] - arrRevenue[month - 2]) / arrRevenue[month - 2]) * 100;

			return new { Total = countOrder, Statistical = arrRevenue, Diff = $"{diff:F2}" };
		}
		public async Task<ResponseDTO> GetStatistical()
		{
			var users =await _userRepository.GetAllUserAccount(1, 10000, "", "ID");
			return new ResponseDTO()
			{
				Data = new
				{
					User = await GetStatisticalUser(),
					Order = await GetStatisticalOrder(),
					Revenue = await GetStatisticalRevenue(),
				}
			};
		}
	}
}
