namespace Deloprosit.Server.Models
{
    public class ChapterSearchResultModel
    {
        public int? ChapterId { get; set; }
        public int? ThemeId { get; set; }
        public string? ThemeTitle { get; set; }
        public DateTime? DateCreated { get; set; }
        public string? SearchFragment { get; set; }
    }
}
