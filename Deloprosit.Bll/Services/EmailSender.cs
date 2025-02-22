using Azure.Communication.Email;

using Microsoft.Extensions.Configuration;

namespace Deloprosit.Bll.Services
{
    public class EmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
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
