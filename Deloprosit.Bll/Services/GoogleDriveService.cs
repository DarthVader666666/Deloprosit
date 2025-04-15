using Google.Apis.Drive.v3;

namespace Deloprosit.Bll.Services
{
    public class GoogleDriveService
    {
        private readonly DriveService _driveService;
        const string folderMimeType = "application/vnd.google-apps.folder";

        public GoogleDriveService(DriveService driveService)
        {
            _driveService = driveService;
        }

        public void RestoreAllDocuments()
        {
            if (!Directory.Exists(ConfigurationHelper.DocsPath))
            {
                Directory.CreateDirectory(ConfigurationHelper.DocsPath ?? throw new ArgumentNullException(nameof(ConfigurationHelper.DocsPath), "Путь к папке документов null"));
            }

            try
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                DownloadFolderContentsAsync(ConfigurationHelper.GoogleDriveFolderId, ConfigurationHelper.DocsPath);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Delete(string? path)
        {
            var id = await GetIdAsync(path);
            var request = _driveService.Files.Delete(id);
            request.SupportsAllDrives = true;

            var permissions = await _driveService.Permissions.List(id).ExecuteAsync();

            var result = request.Execute();
        }

        private async Task<string?> GetIdAsync(string? path)
        {
            var files = await GetFileListAsync();

            if (Directory.Exists(path))
            {
                var folderName = Path.GetDirectoryName(path)?.Split('\\').Last();
                return files.FirstOrDefault(x => x.MimeType == folderMimeType && x.Name == folderName)?.Id;
            }
            else if (File.Exists(path))
            {
                var fileName = Path.GetFileName(path);
                return files.FirstOrDefault(x => x.MimeType != folderMimeType && x.Name == fileName)?.Id;
            }
            else 
            {
                return null;
            }
        }

        private async Task<IList<Google.Apis.Drive.v3.Data.File>> GetFileListAsync()
        {
            var request = _driveService.Files.List();
            request.Q = $"'{ConfigurationHelper.GoogleDriveFolderId}' in parents and trashed = false";
            request.Fields = "files(id, name, mimeType)";
            var result = await request.ExecuteAsync();

            return result.Files;
        }

        private async Task LoopFolderContentsAsync(string? folderId, Action driveAction)
        {
            var request = _driveService.Files.List();
            request.Q = $"'{folderId}' in parents and trashed = false";
            request.Fields = "files(id, name, mimeType)";
            var result = await request.ExecuteAsync();

            foreach (var item in result.Files)
            {
                await LoopFolderContentsAsync(item.Id, driveAction);
            }
        }

        public async Task DownloadFolderContentsAsync(string? folderId, string? localPath)
        {
            var request = _driveService.Files.List();
            request.Q = $"'{folderId}' in parents and trashed = false";
            request.Fields = "files(id, name, mimeType)";
            var result = await request.ExecuteAsync();

            foreach (var item in result.Files)
            {
                var itemPath = Path.Combine(localPath ?? "", item.Name);

                if (item.MimeType == "application/vnd.google-apps.folder")
                {
                    Directory.CreateDirectory(itemPath);
                    await DownloadFolderContentsAsync(item.Id, itemPath);
                }
                else
                {
                    using var fileStream = new FileStream(itemPath, FileMode.Create, FileAccess.Write);
                    await _driveService.Files.Get(item.Id).DownloadAsync(fileStream);
                }
            }
        }
    }
}