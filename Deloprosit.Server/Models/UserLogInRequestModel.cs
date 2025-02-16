namespace Deloprosit.Server.Models
{
    public class UserLogInRequestModel
    {
        public string? NicknameOrEmail { get; set; }
        public string? Password { get; set; }
    }
}
