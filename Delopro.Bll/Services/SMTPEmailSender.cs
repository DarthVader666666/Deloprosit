using System.Net;
using System.Net.Mail;

using Delopro.Bll.Interfaces;

namespace Delopro.Bll.Services
{
    public class SMTPEmailSender : IEmailSender
    {
        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var fromAddress = new MailAddress("postmaster@delopro.site", "Your App Name");
                var toAddress = new MailAddress(to);

                var smtp = new SmtpClient
                {
                    Host = "mailbe07.hoster.by", // Or your SMTP provider
                    Port = 587, // Typically 587 for TLS, 465 for SSL
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("airlex34@gmail.com", "Uchitel15!")
                };

                var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                smtp.Send(message);

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }            
        }
    }
}
