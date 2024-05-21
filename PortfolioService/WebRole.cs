using System;
using System.Collections.Generic;
using System.Linq;
using HealthMonitoringWCFInterface;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace PortfolioService {
    public class WebRole : RoleEntryPoint {
        HealthMonitoringServer healthMonitoringServer;

        public override bool OnStart() {
            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            healthMonitoringServer = new HealthMonitoringServer();
            healthMonitoringServer.Open();

            return result;
        }

        public override void OnStop() {
            base.OnStop();

            healthMonitoringServer.Close();
        }
    }
}
