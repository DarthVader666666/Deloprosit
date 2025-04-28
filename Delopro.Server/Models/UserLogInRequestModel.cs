namespace Delopro.Server.Models
{
    public class UserLogInRequestModel
    {
        public string? Email { get; set; }
        public byte[]? Password { get; set; }
    }
}
