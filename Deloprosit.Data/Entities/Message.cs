namespace Deloprosit.Data.Entities
{
    public class Message
    {
        public int MessageId { get; set; }
        public DateTime DateSent { get; set; }
        public bool IsRead { get; set; } = false;
        public string? Text { get; set; }
    }
}
