using Google;
using Google.Apis.Drive.v3;

using Microsoft.AspNetCore.StaticFiles;

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
                DownloadFolderContentsAsync(ConfigurationHelper.GoogleDriveFolderId, ConfigurationHelper.DocsPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Delete(string? path, bool isFolder = false)
        {
            string? id;

            if (isFolder)
            {
                id = GetId(path?.Split('\\').Last(), isFolder: true);
            }
            else
            {
                var parentFolderName = path?.Split('\\')[..^1].Last();

                var folderId = parentFolderName == ConfigurationHelper.DocsFolderName
                    ? ConfigurationHelper.GoogleDriveFolderId
                    : GetId(parentFolderName, isFolder: true);

                id = GetId(Path.GetFileName(path), folderId);
            }

            var request = _driveService.Files.Delete(id);
            request.SupportsAllDrives = true;

            request.Execute();
        }

        public void CreateFolder(string? folderName)
        {
            var folderMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = folderName,
                MimeType = folderMimeType,
                Parents = [ ConfigurationHelper.GoogleDriveFolderId ]
            };

            try
            {
                var request = _driveService.Files.Create(folderMetadata);
                request.Fields = "id, name, webViewLink";
                var folder = request.Execute();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateFile(string? filePath)
        {
            try
            {
                Delete(filePath);
            }
            catch (GoogleApiException ex)
            {            
            }

            var fileName = Path.GetFileName(filePath);
            var parentFolderName = filePath?.Split('\\')[..^1].Last();

            var folderId = parentFolderName == ConfigurationHelper.DocsFolderName 
                ? ConfigurationHelper.GoogleDriveFolderId 
                : GetId(parentFolderName, isFolder: true);

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = fileName ?? Path.GetFileName(filePath),
                Parents = folderId != null ? new List<string> { folderId } : null
            };

            using var stream = new FileStream(filePath ?? "", FileMode.Open);
            var mimeType = GetMimeType(filePath);

            var request = _driveService.Files.Create(fileMetadata, stream, mimeType);
            request.Fields = "id, name, size, mimeType, webViewLink";

            var uploadProgress = request.Upload();

            if (uploadProgress.Status == Google.Apis.Upload.UploadStatus.Failed)
            {
                throw new Exception($"Загрузка файла {uploadProgress.Exception.Message} в облачное хранилище не удалась");
            }
        }

        public async Task DownloadFolderContentsAsync(string? folderId, string? localPath)
        {
            var files = GetFileList(folderId);

            foreach (var item in files)
            {
                var itemPath = Path.Combine(localPath ?? "", item.Name);

                if (item.MimeType == folderMimeType)
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

        private string? GetId(string? name, string? folderId = null, bool isFolder = false)
        {
            try
            {
                var files = GetFileList(folderId);

                if (isFolder)
                {
                    return files.FirstOrDefault(x => x.MimeType == folderMimeType && x.Name == name)?.Id;
                }
                else
                {
                    return files.FirstOrDefault(x => x.MimeType != folderMimeType && x.Name == name)?.Id;
                }
            }
            catch
            {
                return null;
            }            
        }

        private IList<Google.Apis.Drive.v3.Data.File> GetFileList(string? folderId = null)
        {
            var request = _driveService.Files.List();
            request.Q = $"'{(folderId == null ? ConfigurationHelper.GoogleDriveFolderId : folderId)}' in parents and trashed = false";
            request.Fields = "files(id, name, mimeType)";
            var result = request.Execute();

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

        private static string GetMimeType(string? filePath)
        {
            var mimeProvider = new FileExtensionContentTypeProvider();

            return mimeProvider.TryGetContentType(filePath, out var mimeType)
                ? mimeType
                : "application/octet-stream";
        }  
    }
}