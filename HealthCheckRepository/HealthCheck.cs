using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringService {
    public enum HealthCheckPartition { PortfolioServicePartition, NotificationServicePartition }
    public class HealthCheck : TableEntity {
        public HealthCheck(bool isAlive, HealthCheckPartition healthCheckPartition) {
            this.PartitionKey = healthCheckPartition.ToString();
            this.RowKey = Guid.NewGuid().ToString();
            this.IsAlive = isAlive;
        }

        public HealthCheck() { }

        public bool IsAlive { get; set; }
    }
}
