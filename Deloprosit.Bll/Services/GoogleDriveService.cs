using System.Text;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

using Microsoft.Extensions.Configuration;

namespace Deloprosit.Bll.Services
{
    public class GoogleDriveService
    {
        private readonly string? _token;
        private readonly string? _appName;

        private UserCredential _credentials;
        private readonly string[]? _scopes = { DriveService.Scope.Drive };

        private DriveService? _driveService;

        public GoogleDriveService(IConfiguration config)
        {
            _token = config["GoogleDriveToken"];
            _appName = config["GoogleCloudAppName"];
        }

        public async Task<bool> Start() 
        {
            var credentialPath = Path.Combine(Environment.CurrentDirectory, ".credentials", _appName ?? "");

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(_token ?? ""));

            try
            {
                _credentials = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    clientSecrets: GoogleClientSecrets.FromStream(stream).Secrets,
                    scopes: _scopes,
                    user: "user",
                    taskCancellationToken: CancellationToken.None,
                    new FileDataStore(credentialPath, true)
                );

                _driveService = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = _credentials,
                    ApplicationName = _appName,
                });

                var request = _driveService.Files.List();
                var response = request.Execute();
            }
            catch (Exception ex)
            {
            }
            

            return true;
        }
    }
}
