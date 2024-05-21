using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringWCFInterface {
    public class HealthMonitoringServer {
        private ServiceHost serviceHost;
        private string endpointName = "health-monitoring";  

        public HealthMonitoringServer() {
            RoleInstanceEndpoint endpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints[endpointName];
            string endpointIPAndPortNumber = endpoint.IPEndpoint.ToString();
            string endpointAddress = string.Format("net.tcp://{0}/{1}", endpointIPAndPortNumber, endpointName);

            serviceHost = new ServiceHost(typeof(HealthMonitoring));
            serviceHost.AddServiceEndpoint(typeof(IHealthMonitoring), new NetTcpBinding(), endpointAddress);
        }

        public void Open() {
            try {
                serviceHost.Open();
                Trace.TraceInformation(string.Format("Host for {0} endpoint type opened successfully at {1}", endpointName, DateTime.Now));
            } catch (Exception e) {
                Trace.TraceInformation("Host open error for {0} endpoint type. Error message is: {1}. ", endpointName, e.Message);
            }
        }

        public void Close() {
            try {
                serviceHost.Close();
                Trace.TraceInformation(string.Format("Host for {0} endpoint type closed successfully at {1}", endpointName, DateTime.Now));
            } catch (Exception e) {
                Trace.TraceInformation("Host close error for {0} endpoint type. Error message is: {1}. ", endpointName, e.Message);
            }
        }
    }
}
