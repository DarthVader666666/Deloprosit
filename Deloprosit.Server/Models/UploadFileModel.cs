namespace Deloprosit.Server.Models
{
    public class UploadFileModel
    {
        public List<IFormFile>? Files { get; set; }  
        public string? FolderName { get; set; }
    }
}
