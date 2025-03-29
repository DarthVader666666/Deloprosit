namespace Deloprosit.Server.Models
{
    public class DocumentNode
    {
        public string? Key { get; set; }
        public TreeNode? Data { get; set; }
        public string? Icon { get; set; }
        public ICollection<DocumentNode>? Children { get; set; }
    }
}
