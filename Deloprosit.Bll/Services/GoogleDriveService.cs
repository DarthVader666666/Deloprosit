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

        public async Task Delete(string? name, bool isFolder = false)
        {
            var id = await GetIdAsync(name, isFolder);
            var request = _driveService.Files.Delete(id);
            request.SupportsAllDrives = true;

            var permissions = await _driveService.Permissions.List(id).ExecuteAsync();

            var result = request.Execute();
        }

        public async Task CreateFolderAsync(string? path)
        {
            var folderName = GetName(path).FolderName;

            var folderMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = folderName,
                MimeType = "application/vnd.google-apps.folder",
                Parents = new List<string> { ConfigurationHelper.GoogleDriveFolderId ?? "" }
            };

            try
            {
                var request = _driveService.Files.Create(folderMetadata);
                request.Fields = "id, name, webViewLink";
                var folder = await request.ExecuteAsync();
            }
            catch (Exception ex)
            {
                throw ex;   
            }
        }

        public async Task CreateFileAsync(string? filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var parentFolderName = filePath?.Split('\\')[..^1].Last();

            var folderId = parentFolderName == ConfigurationHelper.DocumentsDirectoryName 
                ? ConfigurationHelper.GoogleDriveFolderId 
                : await GetIdAsync(parentFolderName, isFolder: true);

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = fileName ?? Path.GetFileName(filePath),
                Parents = folderId != null ? new List<string> { folderId } : null
            };

            try
            {
                using var stream = new FileStream(filePath ?? "", FileMode.Open);
                var mimeType = GetMimeType(filePath);

                var request = _driveService.Files.Create(fileMetadata, stream, mimeType);
                request.Fields = "id, name, size, mimeType, webViewLink";
                // For large files, use resumable upload
                //request.ChunkSize = 2 * 256 * 1024; // 2Mb

                var uploadProgress = await request.UploadAsync(CancellationToken.None);

                if (uploadProgress.Status == Google.Apis.Upload.UploadStatus.Failed)
                {
                    throw new Exception($"Upload failed: {uploadProgress.Exception.Message}");
                }

                var file = request.ResponseBody;
                var id = file.Id;
            }
            catch (Exception ex)
            {
            }
            
        }

        public async Task DownloadFolderContentsAsync(string? folderId, string? localPath)
        {
            var files = await GetFileListAsync(folderId);

            foreach (var item in files)
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

        private async Task<string?> GetIdAsync(string? name, bool isFolder = false)
        {
            try
            {
                var files = await GetFileListAsync();

                if (isFolder)
                {
                    return files.FirstOrDefault(x => x.MimeType == folderMimeType && x.Name == name)?.Id;
                }
                else
                {
                    return files.FirstOrDefault(x => x.MimeType != folderMimeType && x.Name == name)?.Id;
                }
            }
            catch (Exception ex)
            {
                return null;
            }            
        }

        private static (string? FileName, string? FolderName) GetName(string? path)
        {
            (string? FileName, string? FolderName) name = (null, null);

            if (Directory.Exists(path))
            {                
                var folderName = path.Split('\\').Last();
                name.FolderName = folderName;
            }
            else if (File.Exists(path))
            {
                var fileName = Path.GetFileName(path);
                name.FileName = fileName;
            }

            return name;
        }

        private async Task<IList<Google.Apis.Drive.v3.Data.File>> GetFileListAsync(string? folderId = null)
        {
            var request = _driveService.Files.List();
            request.Q = $"'{(folderId == null ? ConfigurationHelper.GoogleDriveFolderId : folderId)}' in parents and trashed = false";
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

        private string GetMimeType(string? filePath)
        {
            var mimeProvider = new FileExtensionContentTypeProvider();

            return mimeProvider.TryGetContentType(filePath, out var mimeType)
                ? mimeType
                : "application/octet-stream";
        }        
    }
}