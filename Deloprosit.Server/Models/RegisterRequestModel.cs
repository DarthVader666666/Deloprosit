namespace Deloprosit.Server.Models
{
    public class RegisterRequestModel
    {
        public int[]? Nickname { get; set; }
        public string? Email { get; set; }
        public int[]? FirstName { get; set; }
        public int[]? LastName { get; set; }
        public int[]? Password { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? RegisterDate { get; set; }
        public int[]? Country { get; set; }
        public int[]? City { get; set; }
        public int[]? Info { get; set; }
    }
}
