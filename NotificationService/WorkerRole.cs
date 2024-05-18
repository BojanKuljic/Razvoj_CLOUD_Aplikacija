using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace NotificationService
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private readonly Notification _notificationService;
        private readonly CloudQueue _queue;

        public WorkerRole()
        {
            string sendGridApiKey = CloudConfigurationManager.GetSetting("SendGridApiKey");
            _notificationService = new Notification(sendGridApiKey);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            _queue = queueClient.GetQueueReference("alarms");
            _queue.CreateIfNotExists();
        }

        public override void Run()
        {
            Trace.TraceInformation("NotificationService is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("NotificationService has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("NotificationService is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("NotificationService has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true"); //var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("alarms");
            queue.CreateIfNotExists();

            while (!cancellationToken.IsCancellationRequested)
            {
                //KUPI 20 poruka     TODOO: TREBA PROVERITI PORUKE!!!
                IEnumerable<CloudQueueMessage> messages = await queue.GetMessagesAsync(20);
                foreach (CloudQueueMessage message in messages)
                {   //LOMI IH
                    string[] alarmIds = message.AsString.Split(',');
                    foreach (var alarmId in alarmIds)
                    {
                        //SALJE MAIL
                        await _notificationService.SendEmailAsync("mile.nalog6@gmail.com", "Alarm Triggered", "Your alarm was triggered.");
                    }

                    await queue.DeleteMessageAsync(message);
                }
                //Svakih 10 sekundi
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            }
        }
    }
}
