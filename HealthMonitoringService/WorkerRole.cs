using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using HealthMonitoringConsoleApp;
using HealthMonitoringWCFInterface;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

namespace HealthMonitoringService {
    public class WorkerRole : RoleEntryPoint {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private bool serviceJustDiedPortfolio = false;
        private bool serviceJustDiedNotification = false;
        private readonly IHealthConsoleService consoleService = new ChannelFactory<IHealthConsoleService>(
        new NetTcpBinding(),
        new EndpointAddress("net.tcp://localhost:8000/ConsoleService")).CreateChannel();
        Random random = new Random();

        WCFChannel wcfChannel = new WCFChannel();
        HealthCheckRepository repo = new HealthCheckRepository();

        public override void Run() {
            Trace.TraceInformation("HealthMonitoringService is running");

            try {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            } finally {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart() {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("HealthMonitoringService has been started");

            return result;
        }

        public override void OnStop() {
            Trace.TraceInformation("HealthMonitoringService is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("HealthMonitoringService has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken) {
            while (!cancellationToken.IsCancellationRequested) {
                var portfolioServiceInstances = RoleEnvironment.Roles["PortfolioService"].Instances;
                var notificationServiceInstances = RoleEnvironment.Roles["NotificationService"].Instances;

                int portfolioServiceInstanceIndex = random.Next(portfolioServiceInstances.Count);
                int notificationServiceInstanceIndex = random.Next(notificationServiceInstances.Count);
                RoleInstance portfolioServiceInstance = portfolioServiceInstances[portfolioServiceInstanceIndex];
                RoleInstance notificationServiceInstance = notificationServiceInstances[notificationServiceInstanceIndex];

                if (wcfChannel.HealthCheck(portfolioServiceInstance.InstanceEndpoints["health-monitoring"].IPEndpoint.ToString())) {
                    Trace.TraceInformation("PortfolioService instance " + portfolioServiceInstanceIndex + " is alive");
                    repo.Create(new HealthCheck(true, HealthCheckPartition.PortfolioServicePartition));
                    serviceJustDiedPortfolio = false;
                } else {
                    if(serviceJustDiedPortfolio == false)
                    {
                        Trace.TraceInformation("PortfolioService instance " + portfolioServiceInstanceIndex + " is dead (!)");
                        repo.Create(new HealthCheck(false, HealthCheckPartition.PortfolioServicePartition));
                        serviceJustDiedPortfolio = await consoleService.SendEmails("PortfolioService error!", "PortfolioService instance " + portfolioServiceInstanceIndex + " is dead (!)");
                    }
                    else
                    {
                        Trace.TraceInformation("PortfolioService instance " + portfolioServiceInstanceIndex + " is dead (!)");
                        repo.Create(new HealthCheck(false, HealthCheckPartition.PortfolioServicePartition));
                    }
                }

                if (wcfChannel.HealthCheck(notificationServiceInstance.InstanceEndpoints["health-monitoring"].IPEndpoint.ToString())) {
                    Trace.TraceInformation("NotificationService instance " + notificationServiceInstanceIndex + " is alive");
                    repo.Create(new HealthCheck(true, HealthCheckPartition.NotificationServicePartition));
                    serviceJustDiedNotification = false;
                } else {
                    if (serviceJustDiedNotification == false)
                    {
                        Trace.TraceInformation("NotificationService instance " + notificationServiceInstanceIndex + " is dead (!)");
                        repo.Create(new HealthCheck(false, HealthCheckPartition.NotificationServicePartition));
                        serviceJustDiedNotification = await consoleService.SendEmails("NotificationService error!", "NotificationService instance " + portfolioServiceInstanceIndex + " is dead (!)"); ;
                    }
                    else
                    {
                        Trace.TraceInformation("NotificationService instance " + notificationServiceInstanceIndex + " is dead (!)");
                        repo.Create(new HealthCheck(false, HealthCheckPartition.NotificationServicePartition));
                    }
                }

                await Task.Delay(5000);
            }
        }
    }
}
