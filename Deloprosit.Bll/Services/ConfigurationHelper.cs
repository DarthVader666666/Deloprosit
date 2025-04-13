using Microsoft.Extensions.Configuration;

namespace Deloprosit.Bll
{
    public static class ConfigurationHelper
    {
        public static IConfiguration? Configuration;
        public static string WebRootPath;
        public static string? DocsPath;
        public static string? DocumentsDirectoryName;
        public static string? DocumentsDirectoryId;

        public static void Initialize(IConfiguration configuration, string webRootPath)
        {
            Configuration = configuration;
            DocumentsDirectoryName = configuration["DocumentsDirectoryName"];
            WebRootPath = webRootPath;
            DocsPath = webRootPath + $"\\{DocumentsDirectoryName}\\";
            DocumentsDirectoryId = Configuration["Google:FolderId"];
        }
    }
}
