namespace Delopro.Server.Models
{
    public class DirectoryNode
    {
        public string? Key { get; set; }
        public TreeNode? Data { get; set; }
        public string? Icon { get; set; } = "pi pi-folder";
        public ICollection<DocumentNode>? Children { get; set; }
    }
}
