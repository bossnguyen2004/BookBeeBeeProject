namespace BookBee.DTO.Account
{
    public class ChangePasswordDTO
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string CfPassword { get; set; }
    }
}
