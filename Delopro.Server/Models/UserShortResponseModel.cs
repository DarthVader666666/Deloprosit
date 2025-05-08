namespace Delopro.Server.Models
{
    public class UserShortResponseModel
    {
        public int UserId { get; set; }
        public byte[]? Avatar { get; set; }
        public string? Nickname { get; set; }
        public DateTime? RegisterDate { get; set; }
        public string? Roles { get; set; }
        public int? Status { get; set; }
    }
}
