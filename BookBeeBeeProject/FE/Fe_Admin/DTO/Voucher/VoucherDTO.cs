namespace Fe_Admin.DTO.Voucher
{
    public class VoucherDTO
    {
        public int Id { get; set; }
        public string? VoucherCode { get; set; }
        public string? VoucherName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? DiscountValue { get; set; }
        public int? Status { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
