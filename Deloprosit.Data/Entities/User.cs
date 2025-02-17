namespace Deloprosit.Data.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string? Nickname { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? RegisterDate { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? UserTitle { get; set; }
        public string? Info { get; set; }
        public byte[]? Avatar { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public virtual ICollection<UserRole>? UserRoles { get; set; }
        public virtual ICollection<Chapter>? Chapters { get; set; }
        public virtual ICollection<Theme>? Themes { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
    }
}
