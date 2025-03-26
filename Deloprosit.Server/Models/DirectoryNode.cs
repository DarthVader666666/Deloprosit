namespace Deloprosit.Server.Models
{
    public class DirectoryNode
    {
        public string? Key { get; set; }
        public string? Label { get; set; }
        public string? Data { get; set; }
        public string? Icon { get; set; } = "pi pi-folder";
        public ICollection<DocumentNode>? Children { get; set; }
    }
}
