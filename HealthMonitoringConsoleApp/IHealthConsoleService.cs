using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringConsoleApp
{
    [ServiceContract]
    public interface IHealthConsoleService
    {
        [OperationContract]
        Task<bool> SendEmails(string emailSubject, string emailContent);
    }
}
