namespace Deloprosit.Server.Models
{
    public class RegisterRequestModel
    {
        public byte[]? Nickname { get; set; }
        public string? Email { get; set; }
        public byte[]? FirstName { get; set; }
        public byte[]? LastName { get; set; }
        public byte[]? Password { get; set; }
        public byte[]? Title { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? RegisterDate { get; set; }
        public byte[]? Country { get; set; }
        public byte[]? City { get; set; }
        public byte[]? Info { get; set; }
    }
}
