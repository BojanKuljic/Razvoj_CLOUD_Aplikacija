using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortfolioServiceStorage.TableEntityClasses;

namespace PortfolioServiceStorage.Repos {
    public class CryptocurrencyRepository {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;

        public CryptocurrencyRepository() {
            _storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true"); //var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("CryptocurrencyTable");
            _table.CreateIfNotExists();
        }

        public void Create(Cryptocurrency cryptocurrency) {
            Loop:
            try {
                TableOperation insertOperation = TableOperation.Insert(cryptocurrency);
                _table.Execute(insertOperation);
            } catch {
                cryptocurrency.RowKey = Guid.NewGuid().ToString();
                goto Loop;
            }
        }
    }
}
