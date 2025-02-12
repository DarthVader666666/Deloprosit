namespace Deloprosit.Data.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string? Nickname { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Title { get; set; }
        public string? AdditionalComments { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public byte[]? Avatar { get; set; }
    }
}
