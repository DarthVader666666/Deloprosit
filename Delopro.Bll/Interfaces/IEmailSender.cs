namespace Delopro.Bll.Interfaces
{
    public interface IEmailSender
    {
        bool SendEmail(string to, string subject, string body);
    }
}
