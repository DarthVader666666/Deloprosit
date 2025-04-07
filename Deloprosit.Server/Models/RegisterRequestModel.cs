namespace Deloprosit.Server.Models
{
    public class RegisterRequestModel
    {
        public byte[]? Nickname { get; set; }
        public string? Email { get; set; }
        public byte[]? FirstName { get; set; }
        public byte[]? Password { get; set; }
        public DateTime? RegisterDate { get; set; }
    }
}
