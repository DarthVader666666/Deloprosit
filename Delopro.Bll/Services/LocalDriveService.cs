using Delopro.Bll.Interfaces;

namespace Delopro.Bll.Services
{
    public class LocalDriveService : IDriveService
    {
        public void CreateFile(string? filePath)
        {
            throw new NotImplementedException();
        }

        public void CreateFolder(string folderPath)
        {
            throw new NotImplementedException();
        }

        public void Delete(string? path, bool isFolder = false)
        {
            throw new NotImplementedException();
        }

        public Task DownloadFolderContentsAsync(string? folderId, string? localPath)
        {
            throw new NotImplementedException();
        }

        public void Rename(string? path, string? newName, bool isFolder = false)
        {
            throw new NotImplementedException();
        }

        public void RestoreAllDocuments()
        {
            throw new NotImplementedException();
        }
    }
}
