using System.Net;
using System.Net.Mail;

namespace DotAPicker.Utilities
{
    public static class Email
    {
        private const string SENDER_EMAIL = "daniel@dotapad.com";
        private const string SECRET = "sF2xpEYTrgJ1"; //this only works for this app and won't work for sign-in
        public const string SMTP_HOST = "smtp.zoho.com";
        public const int SMTP_SERVER_PORT = 587;

        public static bool SendEmail(string to, string subject, string body)
        {
            MailMessage msg = new(SENDER_EMAIL, to) {
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            var client = new SmtpClient(SMTP_HOST, SMTP_SERVER_PORT) {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(SENDER_EMAIL, SECRET),
                EnableSsl = true
            };

            try
            {
                client.Send(msg);
                return true;
            }
            catch (Exception) { return false; }
        }
    }
}