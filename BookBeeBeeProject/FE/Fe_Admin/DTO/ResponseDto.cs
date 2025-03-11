namespace Fe_Admin.DTO
{
    public class ResponseDto
    {
        public object? Content { get; set; }
        public bool IsSuccess { get; set; } = true;
        public int Code { get; set; } = 200;
        public string Message { get; set; } = "Thành công";
        public int Count { get; set; } = 0;
        public int TotalPage { get; set; } = 0;
        public PagingInfo? PagingInfo { get; set; }
    }
}
