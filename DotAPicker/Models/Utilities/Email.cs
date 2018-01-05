using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace DotAPicker.Models.Utilities
{
    public class Email
    {
        public static bool SendEmail(string to, string subject, string body)
        {
            MailMessage msg = new MailMessage("daniel@dotapad.com", to);
            msg.Subject = subject;
            msg.Body = body;
            msg.IsBodyHtml = false;

            var client = new SmtpClient("smtp.zoho.com", 465);
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("daniel@dotapad.com", "Dizz0ap4d^^");
            client.EnableSsl = true;

            try
            {
                client.Send(msg);
                return true;
            }
            catch (Exception) { return false; }
        }
    }
}