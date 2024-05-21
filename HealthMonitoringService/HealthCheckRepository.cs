using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringService {
    public class HealthCheckRepository {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;

        public HealthCheckRepository() {
            _storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true"); //var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("HealthCheckTable");
            _table.CreateIfNotExists();
        }

        public void Create(HealthCheck healthCheck) {
            Loop:
            try {
                TableOperation insertOrMergeOperation = TableOperation.Insert(healthCheck);
                _table.Execute(insertOrMergeOperation);
            } catch {
                healthCheck.RowKey = Guid.NewGuid().ToString();
                goto Loop;
            }
            
        }
    }
}
