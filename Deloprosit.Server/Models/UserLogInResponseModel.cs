namespace Deloprosit.Server.Models
{
    public class UserLogInResponseModel
    {
        public string? Nickname { get; set; }
        public string[]? Roles { get; set; }
        public bool? IsAuthenticated { get; set; } = false;
        public bool? Remember { get; set; } = false;
    }
}
