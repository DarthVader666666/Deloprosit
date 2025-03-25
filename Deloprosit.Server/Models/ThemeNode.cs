namespace Deloprosit.Server.Models
{
    public class ThemeNode
    {
        public string? Key { get; set; }
        public string? Label { get; set; }
        public string? Data { get; set; }
        public string? Icon { get; set; } = "pi pi-bookmark";
        public string? Type { get; set; } = "url";
    }
}
