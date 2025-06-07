using Delopro.Bll.Interfaces;
using Google;
using Google.Apis.Drive.v3;

using Microsoft.AspNetCore.StaticFiles;

namespace Delopro.Bll.Services
{
    public class GoogleDriveService: IDriveService
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
            string? parentId;
            string? id = GetId(path, out parentId, isFolder);

            var request = _driveService.Files.Delete(id);
            request.SupportsAllDrives = true;

            try
            {
                request.Execute();
            }
            catch (GoogleApiException ex)
            {
                throw ex;
            }
        }

        public void CreateFolder(string folderPath)
        {
            var newFolderName = folderPath.Replace(ConfigurationHelper.DocsPath!, "")
                .TrimStart(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar).Last();

            string? parentId;
            GetId(folderPath, out parentId, isFolder: true);

            var folderMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = newFolderName,
                MimeType = folderMimeType,
                Parents = [ parentId ]
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

        public void CreateFile(string? filePath)
        {
            var fileName = Path.GetFileName(filePath);

            string folderId;
            GetId(filePath, out folderId, isFolder: true);

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

            string parentId;
            var id = GetId(path, out parentId, isFolder);

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

        private string? GetId(string? path, out string? parentId, bool isFolder = false)
        {
            var pathArray = path?.Replace(ConfigurationHelper.DocsPath!, "").TrimStart(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
            string? id = null;
            parentId = ConfigurationHelper.DocsFolderId;

            foreach (var name in pathArray ?? [])
            {
                var files = GetFileList(parentId);
                id = files.FirstOrDefault(x => x.MimeType == folderMimeType && x.Name == name)?.Id;

                if (id != null)
                {
                    parentId = id;
                }
                else if(!isFolder)
                {
                    id = files.FirstOrDefault(x => x.MimeType != folderMimeType && x.Name == name)?.Id ?? id;
                }
            }

            return id;
        }

        private IList<Google.Apis.Drive.v3.Data.File> GetFileList(string? folderId = null)
        {
            var request = _driveService.Files.List();
            request.Q = $"'{folderId ?? ConfigurationHelper.DocsFolderId}' in parents and trashed = false";
            request.Fields = "files(id, name, mimeType)";
            var result = request.Execute();

            return result.Files;
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