namespace Delopro.Bll.Interfaces
{
    public interface IDriveService
    {
        public void RestoreAllDocuments();
        public void Delete(string? path, bool isFolder = false);
        public void CreateFolder(string folderPath);
        public void CreateFile(string? filePath);
        public void Rename(string? path, string? newName, bool isFolder = false);
        public Task DownloadFolderContentsAsync(string? folderId, string? localPath);
    }
}
