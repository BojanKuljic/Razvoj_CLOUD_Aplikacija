using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortfolioServiceStorage.TableEntityClasses;

namespace PortfolioServiceStorage.Repos {
    public class TransactionRepository {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;

        public TransactionRepository() {
            _storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true"); //var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("TransactionTable");
            _table.CreateIfNotExists();
        }

        public void Create(Transaction transaction) {
            Loop:
            try {
                TableOperation insertOperation = TableOperation.Insert(transaction);
                _table.Execute(insertOperation);
            } catch {
                transaction.RowKey = Guid.NewGuid().ToString();
                goto Loop;
            }
        }
    }
}
