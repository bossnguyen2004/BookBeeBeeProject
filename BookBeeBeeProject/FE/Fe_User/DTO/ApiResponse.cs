using Newtonsoft.Json;

namespace Fe_User.DTO
{
	public class ApiResponse<T>
	{
        [JsonProperty("data")]
        public T Data { get; set; } // Danh sách sách từ API

        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
    }
}
