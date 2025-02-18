namespace Deloprosit.Server.Models
{
    public class RegisterRequestModel
    {
        public string? Nickname { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Password { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Info { get; set; }
    }
}
