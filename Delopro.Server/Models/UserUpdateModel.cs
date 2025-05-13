namespace Delopro.Server.Models
{
    public class UserUpdateModel
    {
        public int UserId { get; set; }
        public DateTime? DeletionDate { get; set; }
        public int? Status { get; set; }
        public int[]? Roles { get; set; }
    }
}
