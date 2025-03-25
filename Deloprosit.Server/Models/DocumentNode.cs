namespace Deloprosit.Server.Models
{
    public class DocumentNode
    {
        public string? Key { get; set; }
        public string? Label { get; set; }
        public string? Data { get; set; }
        public string? Icon { get; set; } = "pi pi-file";
        public string? Type { get; set; } = "url";
    }
}
