using Google.Apis.Drive.v3;

namespace Deloprosit.Bll.Services
{
    public class GoogleDriveService
    {
        private readonly DriveService _driveService;

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

        public async Task Delete(string? path)
        {
            var fileName = Path.GetFileNameWithoutExtension(path);
            var folderName = fileName == null ? Path.GetDirectoryName(path)?.Split('\\').Last() : null;

            var request = _driveService.Files.List();
            request.Q = $"name = '{fileName}' and trashed = false";
            request.Fields = "files(id, name)";

            var result = await request.ExecuteAsync();
            var id = result.Files.FirstOrDefault()?.Id;

            try
            {

                var folder = result.Files.FirstOrDefault(x => x.MimeType == "application/vnd.google-apps.folder" && x.Name == folderName);
                var file = result.Files.FirstOrDefault(x => x.MimeType == "application/vnd.google-apps.file" && x.Name == fileName);
            }
            catch (Exception ex)
            {

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