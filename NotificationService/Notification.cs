using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
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
        //METODA ZA SLANJE MAILA SA SENDGRIDOM
        public async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            SendGridClient client = new SendGridClient(_sendGridApiKey);
            EmailAddress from = new EmailAddress("mile.nalog6@gmail.com", "KO JE");
            EmailAddress to = new EmailAddress(recipientEmail);
            SendGridMessage msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);
            Response response = await client.SendEmailAsync(msg);

            if (response.StatusCode != System.Net.HttpStatusCode.OK &&
                response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                throw new Exception($"Failed to send email. Status code: {response.StatusCode}");
            }
        }
    }
}
