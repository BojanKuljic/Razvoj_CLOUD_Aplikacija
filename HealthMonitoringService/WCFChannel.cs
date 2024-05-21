using HealthMonitoringWCFInterface;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringService {
    public class WCFChannel {
        public bool HealthCheck(string endpointIPAndPortNumber) {
            try {
                // Every time a new channel is created so it doesn't communicate with the same process every time
                ChannelFactory<IHealthMonitoring> factory = new ChannelFactory<IHealthMonitoring>(new NetTcpBinding(), new EndpointAddress("net.tcp://" + endpointIPAndPortNumber + "/health-monitoring"));
                IHealthMonitoring channel = factory.CreateChannel();

                channel.HealthCheck();

                return true;
            } catch (Exception e) {
                return false;
            }
        }
    }
}
