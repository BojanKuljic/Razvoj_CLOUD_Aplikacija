using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using System.Diagnostics;

namespace EmailSending {
    /*
        How to set up a google account so it can be used by a third party app:
            Step one: enable 2FA
                https://myaccount.google.com/signinoptions/two-step-verification/enroll-welcome
            Step two: create an app - specific password
                https://myaccount.google.com/apppasswords
            After this, put the sixteen-letter password in Authenticate() method of SmtpClient
    */
    public class EmailSender
    {
        string senderAddress = "throwaway134322312@gmail.com";
        string senderAppPassword = "veqw ptoq qgmb ulwo";
        string smtpServerAddress = "smtp.gmail.com";
        int smtpServerPortNumber = 587;

        public async Task<bool> Send(string receiverAddress, string emailSubject, string emailBody)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(senderAddress));
                email.To.Add(MailboxAddress.Parse(receiverAddress));
                email.Subject = emailSubject;
                email.Body = new TextPart(TextFormat.Plain) { Text = emailBody };

                using (var smtp = new SmtpClient())
                {
                    Trace.TraceInformation($"Connecting to SMTP server: {smtpServerAddress}");
                    await smtp.ConnectAsync(smtpServerAddress, smtpServerPortNumber, SecureSocketOptions.StartTls);
                    Trace.TraceInformation($"Authenticating as: {senderAddress}");
                    await smtp.AuthenticateAsync(senderAddress, senderAppPassword);
                    Trace.TraceInformation($"Sending email to: {receiverAddress}");
                    await smtp.SendAsync(email);
                    await smtp.DisconnectAsync(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error sending email: {ex.Message}");
                return false;
            }
        }
    }

}
