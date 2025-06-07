namespace Delopro.Server.Models
{
    public class DocumentNode
    {
        public string? Key { get; set; }
        public TreeNode? Data { get; set; }
        public string? Icon { get; set; }
        public List<DocumentNode>? Children { get; set; } = [];
    }
}
