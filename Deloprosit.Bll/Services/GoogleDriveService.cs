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
                DownloadFolderContentsAsync(ConfigurationHelper.DocsFolderId, ConfigurationHelper.DocsPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Delete(string? path, bool isFolder = false)
        {
            string? id = GetId(path, isFolder);

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
                Parents = [ ConfigurationHelper.DocsFolderId ]
            };

            try
            {
                var request = _driveService.Files.Create(folderMetadata);
                request.Fields = "id, name, webViewLink";
                request.Execute();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateFile(string? filePath, bool overwrite = false)
        {
            if(overwrite)
            {
                Delete(filePath);
            }

            var fileName = Path.GetFileName(filePath);
            var parentFolderName = filePath?.Split(Path.DirectorySeparatorChar)[..^1].Last();

            var folderId = parentFolderName == ConfigurationHelper.DocsFolderName 
                ? ConfigurationHelper.DocsFolderId 
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
                throw new Exception($"Загрузка файла в облачное хранилище не удалась. {uploadProgress.Exception.Message}");
            }
        }

        public void Rename(string? path, string? newName, bool isFolder = false)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path), "Не указано");
            }

            if (newName == null)
            {
                throw new ArgumentNullException(nameof(newName), "Не указано");
            }

            var id = GetId(path, isFolder);

            var file = _driveService.Files.Get(id).Execute() ?? throw new NullReferenceException($"Файл(папка) не найдена в облачном хранилище");

            file.Id = null;
            file.Kind = null;
            file.MimeType = null;
            file.CreatedTimeDateTimeOffset = null;
            file.ModifiedByMeTimeDateTimeOffset = null;

            file.Name = newName;

            var request = _driveService.Files.Update(file, id);
            request.Execute();
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

        private string? GetId(string? path, bool isFolder = false)
        {
            string? id;

            if (isFolder)
            {
                id = GetIdCore(path?.Split(Path.DirectorySeparatorChar).Last());
            }
            else
            {
                var parentFolderName = path?.Split(Path.DirectorySeparatorChar)[..^1].Last();

                var folderId = parentFolderName == ConfigurationHelper.DocsFolderName
                    ? ConfigurationHelper.DocsFolderId
                    : GetId(parentFolderName, isFolder: true);

                id = GetIdCore(Path.GetFileName(path), folderId);
            }

            return id;


            string? GetIdCore(string? name, string? folderId = null)
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
        }

        private IList<Google.Apis.Drive.v3.Data.File> GetFileList(string? folderId = null)
        {
            var request = _driveService.Files.List();
            request.Q = $"'{folderId ?? ConfigurationHelper.DocsFolderId}' in parents and trashed = false";
            request.Fields = "files(id, name, mimeType)";
            var result = request.Execute();

            return result.Files;
        }

        private async Task LoopFolderContentsAsync(string? folderId, Action driveAction)
        {
            var files = GetFileList(folderId);

            foreach (var item in files)
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