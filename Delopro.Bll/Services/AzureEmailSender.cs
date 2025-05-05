using Azure.Communication.Email;

using Delopro.Bll.Interfaces;

using Microsoft.Extensions.Configuration;

namespace Delopro.Bll.Services
{
    public class AzureEmailSender: IEmailSender
    {
        private readonly IConfiguration _configuration;

        public AzureEmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool SendEmail(string to, string subject, string body)
        {
            var sender = _configuration["AzureEmailSender"];
            var connectionString = _configuration["AzureCommunicationService"];

            var client = new EmailClient(connectionString);

            EmailSendOperation? result;

            try
            {
                result = client.Send(Azure.WaitUntil.Completed, sender, to, subject, body);
            }
            catch
            {
                return false;
            }

            return result?.Value.Status == EmailSendStatus.Succeeded;
        }
    }
}
