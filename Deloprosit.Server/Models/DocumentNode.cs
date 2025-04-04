namespace Deloprosit.Server.Models
{
    public class DocumentNode
    {
        public string? Key { get; set; }
        public TreeNode? Data { get; set; }
        public string? Icon { get; set; }
        public IEnumerable<DocumentNode>? Children { get; set; }
    }
}
