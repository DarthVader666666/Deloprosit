using Microsoft.Extensions.Configuration;

namespace Delopro.Bll
{
    public static class ConfigurationHelper
    {
        public static IConfiguration? Configuration;
        public static string? WebRootPath;
        public static string? DocsPath;
        public static string? DocsFolderName;
        public static string? DocsFolderId;

        public static void Initialize(IConfiguration configuration, string webRootPath)
        {
            Configuration = configuration;
            DocsFolderName = configuration["DocsFolderName"];
            WebRootPath = webRootPath;
            DocsPath = Path.Combine(webRootPath, DocsFolderName ?? string.Empty);
            DocsFolderId = Configuration["GoogleDrive:FolderId"];
        }
    }
}