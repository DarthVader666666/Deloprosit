namespace Deloprosit.Server
{
    public static class ConfigurationHelper
    {
        public static IConfiguration? Configuration;
        public static IWebHostEnvironment? WebHostEnvironment;
        public static string WebRootPath;
        public static string? DocsPath;
        public static string? DocumentsDirectoryName;

        public static void Initialize(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            DocumentsDirectoryName = configuration["DocumentsDirectoryName"];
            WebHostEnvironment = webHostEnvironment;
            WebRootPath = webHostEnvironment.WebRootPath;
            DocsPath = webHostEnvironment.WebRootPath + $"\\{DocumentsDirectoryName}\\";

        }
    }
}
