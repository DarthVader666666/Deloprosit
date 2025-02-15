namespace Deloprosit.Data.Entities
{
    public class Chapter
    {
        public int ChapterId { get; set; }
        public int UserId { get; set; }
        public string? ChapterTitle { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateDeleted { get; set; }
        public User? User { get; set; }
        public ICollection<Theme>? Themes { get; set; }
    }
}
