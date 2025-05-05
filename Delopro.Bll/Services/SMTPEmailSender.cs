using System.Net;
using System.Net.Mail;

using Delopro.Bll.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Delopro.Bll.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly CryptoService _cryptoService;

        public SmtpEmailSender(IConfiguration configuration, CryptoService cryptoService)
        {
            _configuration = configuration;
            _cryptoService = cryptoService;
        }

        public bool SendEmail(string to, string subject, string body)
        {
            var fromAddress = new MailAddress(_configuration["SmtpEmailSender:UserName"] ?? "", "DeloPro");
            var toAddress = new MailAddress(to);

            var smtp = new SmtpClient
            {
                Host = _configuration["SmtpEmailSender:Host"] ?? "", // Or your SMTP provider
                Port = 587, // Typically 587 for TLS, 465 for SSL
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_configuration["SmtpEmailSender:UserName"], _cryptoService.Decrypt(_configuration["SmtpEmailSender:Password"]))
            };

            var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            try
            {
                smtp.Send(message);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
