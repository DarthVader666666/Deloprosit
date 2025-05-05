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

        public async Task<bool> SendEmailAsync(string email, string subject, string message)
        {
            var sender = _configuration["AzureEmailSender"];
            var connectionString = _configuration["AzureCommunicationService"];

            var client = new EmailClient(connectionString);

            EmailSendOperation? result = null;

            try
            {
                result = await client.SendAsync(Azure.WaitUntil.Completed, sender, email, subject, message);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            return result?.Value.Status == EmailSendStatus.Succeeded;
        }
    }
}
