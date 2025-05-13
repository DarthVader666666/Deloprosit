namespace Delopro.Data.Entities
{
    public class Message
    {
        public int MessageId { get; set; }
        public int UserId { get; set; } = 1;
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Text { get; set; }
        public DateTime DateSent { get; set; }
        public bool IsRead { get; set; } = false;
        public User? UserReceiver { get; set; }
    }
}
