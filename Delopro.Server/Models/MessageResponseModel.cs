namespace Delopro.Server.Models
{
    public class MessageResponseModel
    {
        public int MessageId { get; set; }
        public string? Name { get; set; }
        public string? Contacts { get; set; }
        public string? Text { get; set; }
        public DateTime DateSent { get; set; }
        public bool IsRead { get; set; }
    }
}
