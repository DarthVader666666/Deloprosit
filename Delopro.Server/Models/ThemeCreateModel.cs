namespace Delopro.Server.Models
{
    public class ThemeCreateModel
    {
        public int? ThemeId { get; set; }
        public int? UserId { get; set; }
        public int? ChapterId { get; set; }
        public string? ThemeTitle { get; set; }
        public string? Content { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateDeleted { get; set; }
    }
}
