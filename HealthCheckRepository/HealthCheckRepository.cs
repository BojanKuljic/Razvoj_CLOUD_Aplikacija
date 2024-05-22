using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

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
                TableOperation insertOperation = TableOperation.Insert(healthCheck);
                _table.Execute(insertOperation);
            } catch {
                healthCheck.RowKey = Guid.NewGuid().ToString();
                goto Loop;
            }
        }

        public IEnumerable<HealthCheck> ReadPortfolioServiceHealthChecksFromLast24Hours() {
            DateTime last24Hours = DateTime.UtcNow.AddHours(-24);
            string partitionFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, HealthCheckPartition.PortfolioServicePartition.ToString());
            string timeFilter = TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.GreaterThanOrEqual, last24Hours);

            string combinedFilter = TableQuery.CombineFilters(partitionFilter, TableOperators.And, timeFilter);

            TableQuery<HealthCheck> query = new TableQuery<HealthCheck>().Where(combinedFilter);

            return _table.ExecuteQuery(query);
        }

        public IEnumerable<HealthCheck> ReadPortfolioServiceHealthChecksFromLastHour() {
            DateTime last24Hours = DateTime.UtcNow.AddHours(-1);
            string partitionFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, HealthCheckPartition.PortfolioServicePartition.ToString());
            string timeFilter = TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.GreaterThanOrEqual, last24Hours);

            string combinedFilter = TableQuery.CombineFilters(partitionFilter, TableOperators.And, timeFilter);

            TableQuery<HealthCheck> query = new TableQuery<HealthCheck>().Where(combinedFilter);

            var result = _table.ExecuteQuery(query).OrderBy(h => h.Timestamp);

            return result;
        }
    }
}
