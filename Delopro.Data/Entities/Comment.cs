namespace Delopro.Data.Entities
{
    public class Comment
    {
        public int CommentId { get; set; }
        public int UserId { get; set; }
        public int ThemeId { get; set; }
        public string? Text { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateEdited { get; set; }
        public DateTime? DateDeleted { get; set;}
        public User? User { get; set; }
        public Theme? Theme { get; set; }
    }
}
