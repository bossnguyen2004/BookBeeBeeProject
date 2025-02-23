namespace BookBee.DTO.Response
{
    public class ResponseDTO
    {
        public object? Content { get; set; }
        public bool IsSuccess { get; set; } = true;
        public int Code { get; set; } = 200;
        public string Message { get; set; } = "Success";
        public int Count { get; set; } = 0;
        public int Total { get; set; } = 0;
        public Object Data { get; set; } = null;
    }
}
