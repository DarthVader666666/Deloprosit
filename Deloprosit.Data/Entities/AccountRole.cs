namespace Deloprosit.Data.Entities
{
    public class AccountRole
    {
        public int RoleId { get; set; }
        public int AccountId { get; set; }
        public Account? Account { get; set; }
        public Role? Role { get; set; }
    }
}
