using Delopro.Data.Enums;

namespace Delopro.Data.Entities
{
    public class UserRole
    {
        public int RoleId { get; set; } = (int)UserRoleType.User;
        public int UserId { get; set; }
        public User? User { get; set; }
        public Role? Role { get; set; }
    }
}
