using Deloprosit.Data.Entities;

namespace Deloprosit.Server.Models
{
    public class ChapterResponseModel
    {
        public int ChapterId { get; set; }
        public int UserId { get; set; }
        public string? ChapterTitle { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateDeleted { get; set; }
        public ThemeResponseModel[]? Themes { get; set; } = [];
    }
}
