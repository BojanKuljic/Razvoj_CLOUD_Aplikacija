using EmailSending;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringConsoleApp
{
    public class HealthConsoleService : IHealthConsoleService
    {
        private static readonly EmailSender _sender = new EmailSender();

        public async Task<bool> SendEmails(string emailSubject, string emailContent)
        {
            foreach(var email in HealthConsoleApp.EmailList)
            {
                await _sender.Send(email, emailSubject, emailContent);
            }
            return true;
        }
    }
}
