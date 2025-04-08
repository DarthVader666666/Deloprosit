namespace Deloprosit.Data.Entities
{
    public class Captcha
    {
        public int CaptchaId { get; set; }
        public string? Image { get; set; }
        public string? Code { get; set; }
    }
}
