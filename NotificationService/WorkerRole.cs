using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CryptocurrencyConversion;
using HealthMonitoringWCFInterface;
using Microsoft.Azure;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using EmailSending;
using NotificationService.NotificationUtilities;

namespace NotificationService
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private readonly EmailSender _notificationService;
        private readonly CloudQueue _queue;
        private readonly CryptocurrencyConverter cryptoCurrencyConverter;

        HealthMonitoringServer healthMonitoringServer;
        NotificationRepository notificationRepo = new NotificationRepository();

        public WorkerRole()
        {
            _notificationService = new EmailSender();
            cryptoCurrencyConverter = new CryptocurrencyConverter();
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
                //odkomentarisati ovo za test padanja servisa posle 30 sekundi
                //Task.Delay(TimeSpan.FromSeconds(30)).ContinueWith(t => cancellationTokenSource.Cancel());

                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            catch (AggregateException ae)
            {
                foreach (var ex in ae.InnerExceptions)
                {
                    if (ex is TaskCanceledException)
                    {
                        Trace.TraceInformation("Task was cancelled as expected.");
                    }
                    else
                    {
                        Trace.TraceError("An unexpected exception occurred: {0}", ex);
                    }
                }
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

            healthMonitoringServer = new HealthMonitoringServer();
            healthMonitoringServer.Open();

            Trace.TraceInformation("NotificationService has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("NotificationService is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            healthMonitoringServer.Close();

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
                IEnumerable<CloudQueueMessage> messages = await queue.GetMessagesAsync(20);
                foreach (CloudQueueMessage message in messages)
                {
                    string[] alarmDetails = message.AsString.Split('|');
                    string cryptocurrencyName = alarmDetails[0];
                    string alertThreshold = alarmDetails[1];
                    string email = alarmDetails[2];
                    bool isLowerTreshold = bool.Parse(alarmDetails[3]);
                    double currentCryptoValue = await cryptoCurrencyConverter.ConvertWithCurrentPrice(cryptocurrencyName, "USDT", 1);
                    if (isLowerTreshold && currentCryptoValue < double.Parse(alertThreshold))
                    {
                        await _notificationService.Send(email, $"Alarm Triggered for {cryptocurrencyName}", $"Your alarm was triggered when cryptocurrency: {cryptocurrencyName} fell below {alertThreshold} threshold.");

                        Notification n = new Notification(DateTime.Now.ToString(), $"Your alarm was triggered when cryptocurrency: {cryptocurrencyName} fell below {alertThreshold} threshold.");

                        await queue.DeleteMessageAsync(message);

                        notificationRepo.InsertNotification(n);
                    }
                    else if (!isLowerTreshold && currentCryptoValue > double.Parse(alertThreshold))
                    {
                        await _notificationService.Send(email, $"Alarm Triggered for {cryptocurrencyName}", $"Your alarm was triggered when cryptocurrency: {cryptocurrencyName} reached {alertThreshold} threshold.");

                        Notification n = new Notification(DateTime.Now.ToString(), $"Your alarm was triggered when cryptocurrency: {cryptocurrencyName} reached {alertThreshold} threshold.");

                        await queue.DeleteMessageAsync(message);

                        notificationRepo.InsertNotification(n);
                    }

                }
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            }
        }
    }
}
