namespace Deloprosit.Server
{
    public static class ConfigurationHelper
    {
        public static IConfiguration? Configuration;
        public static IWebHostEnvironment? WebHostEnvironment;
        public static string? WebRootPath;

        public static void Initialize(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
            WebRootPath = webHostEnvironment.WebRootPath + "\\docs\\";
        }
    }
}
