using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HealthMonitoringWCFInterface;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

namespace HealthMonitoringService {
    public class WorkerRole : RoleEntryPoint {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

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
            Random random = new Random();

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
                } else {
                    Trace.TraceInformation("PortfolioService instance " + portfolioServiceInstanceIndex + " is dead (!)");
                    repo.Create(new HealthCheck(false, HealthCheckPartition.PortfolioServicePartition));
                    // TODO: slanje mejla
                }

                if (wcfChannel.HealthCheck(notificationServiceInstance.InstanceEndpoints["health-monitoring"].IPEndpoint.ToString())) {
                    Trace.TraceInformation("NotificationService instance " + portfolioServiceInstanceIndex + " is alive");
                    repo.Create(new HealthCheck(true, HealthCheckPartition.NotificationServicePartition));
                } else {
                    Trace.TraceInformation("NotificationService instance " + portfolioServiceInstanceIndex + " is dead (!)");
                    repo.Create(new HealthCheck(false, HealthCheckPartition.NotificationServicePartition));
                    // TODO: slanje mejla
                }

                await Task.Delay(3000);
            }
        }
    }
}
