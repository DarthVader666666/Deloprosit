namespace Delopro.Server.Models
{
    public class UserLongResponseModel
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
        public int? Status { get; set; }
        public int[]? Roles { get; set; }
    }
}
