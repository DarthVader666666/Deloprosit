namespace Deloprosit.Data.Entities
{
    public class Account
    {
        public int AccountId { get; set; }
        public int UserId { get; set; }
        public string? Nickname { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public User? User { get; set; }
        public virtual ICollection<AccountRole>? AccountRoles { get; set; }
    }
}
