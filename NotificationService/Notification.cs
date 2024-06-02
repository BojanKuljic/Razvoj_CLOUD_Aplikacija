using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService
{
    public class Notification
    {
        private readonly string _sendGridApiKey;

        public Notification(string sendGridApiKey)
        {
            _sendGridApiKey = sendGridApiKey;
        }
        public void SendEmailAsync(string recipientEmail, string subject, string body)
        {
            //SendGridClient client = new SendGridClient(_sendGridApiKey);
            //EmailAddress from = new EmailAddress("mile.nalog6@gmail.com", "KO JE");
            //EmailAddress to = new EmailAddress(recipientEmail);
            //SendGridMessage msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);
            //Response response = await client.SendEmailAsync(msg);

            //if (response.StatusCode != System.Net.HttpStatusCode.OK &&
            //    response.StatusCode != System.Net.HttpStatusCode.Accepted)
            //{
            //    throw new Exception($"Failed to send email. Status code: {response.StatusCode}");
            //}

            try
            {

                var mailMessage = new MailMessage
                {
//                    From = new MailAddress(MAIL),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false,
                };
                mailMessage.To.Add(recipientEmail);
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
//                    Credentials = new NetworkCredential(MAIL, SIFRA),
                    EnableSsl = true,
                };
                smtpClient.Send(mailMessage);
                Trace.TraceInformation("Email sent to: " + subject);
            }
            catch (System.Exception e)
            {
                Trace.TraceError(e.Message);
            }
        }
    }
}
