namespace BookBee.DTO.Response
{
    public class ResponseDTO
    {
        public int Code { get; set; } = 200;
        public string Message { get; set; } = "Success";
        public int Total { get; set; } = 0;
        public Object Data { get; set; } = null;
    }
}
