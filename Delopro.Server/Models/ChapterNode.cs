namespace Delopro.Server.Models
{
    public class ChapterNode
    {
        public string? Key { get; set; }
        public string? Label { get; set; }
        public ICollection<ThemeNode>? Children { get; set; }
    }
}
