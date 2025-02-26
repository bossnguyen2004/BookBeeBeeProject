using BookBee.DTO.Response;

namespace BookBee.Services.StatisticalService
{
	public interface IStatisticalService
	{
		Task<ResponseDTO> GetStatistical();
	}
}
