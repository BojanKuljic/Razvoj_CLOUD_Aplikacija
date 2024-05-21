using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringWCFInterface {
    [ServiceContract]
    public interface IHealthMonitoring {
        [OperationContract]
        void HealthCheck();
    }
}
