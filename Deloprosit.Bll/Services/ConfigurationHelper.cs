using Microsoft.Extensions.Configuration;

namespace Deloprosit.Bll
{
    public static class ConfigurationHelper
    {
        public static IConfiguration? Configuration;
        public static string? WebRootPath;
        public static string? DocsPath;
        public static string? DocsFolderName;
        public static string? GoogleDriveFolderId;

        public static void Initialize(IConfiguration configuration, string webRootPath)
        {
            Configuration = configuration;
            DocsFolderName = configuration["DocumentsDirectoryName"];
            WebRootPath = webRootPath;
            DocsPath = Path.Combine(webRootPath, DocsFolderName ?? string.Empty);
            GoogleDriveFolderId = Configuration["GoogleDrive:FolderId"];
        }
    }
}