namespace Delopro.Data.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string? Nickname { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; } = null;
        public DateTime? BirthDate { get; set; } = null;
        public DateTime? RegisterDate { get; set; }
        public DateTime? DeletionDate { get; set; }
        public string? Country { get; set; } = null;
        public string? City { get; set; } = null;
        public string? UserTitle { get; set; }
        public string? Info { get; set; } = null;
        public byte[]? Avatar { get; set; } = null;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Password { get; set; }
        public bool IsConfirmed { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public virtual ICollection<UserRole>? UserRoles { get; set; }
        public virtual ICollection<Chapter>? Chapters { get; set; }
        public virtual ICollection<Theme>? Themes { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Message>? Messages { get; set; }
    }
}
