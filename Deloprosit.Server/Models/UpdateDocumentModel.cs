namespace Deloprosit.Server.Models
{
    public class UpdateDocumentModel
    {
        public string? NewName { get; set; }
        public string? OldName { get; set; }
        public string? Type { get; set; }
        public string? Path { get; set; }
    }
}
