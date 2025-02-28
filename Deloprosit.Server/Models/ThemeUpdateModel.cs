namespace Deloprosit.Server.Models
{
    public class ThemeUpdateModel
    {
        public int? ThemeId { get; set; }
        public int? UserId { get; set; }
        public int? ChapterId { get; set; }
        public string? Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateDeleted { get; set; }
    }
}
