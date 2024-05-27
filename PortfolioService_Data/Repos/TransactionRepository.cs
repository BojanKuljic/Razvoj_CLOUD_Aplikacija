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

        public IEnumerable<Transaction> ReadUsersTransactions(string userEmail) {
            string filter = TableQuery.GenerateFilterCondition("UserEmail", QueryComparisons.Equal, userEmail);

            TableQuery<Transaction> query = new TableQuery<Transaction>().Where(filter);

            return _table.ExecuteQuery(query);
        }

        public void Delete(string cryptocurrencyName, string transactionType, double transactionAmountUSD, double transactionAmountCrypto, string transactionDateAndTime, string userEmail) {
            string filter1 = TableQuery.GenerateFilterCondition("CryptocurrencyName", QueryComparisons.Equal, cryptocurrencyName);
            string filter2 = TableQuery.GenerateFilterCondition("Type", QueryComparisons.Equal, transactionType);
            string filter3 = TableQuery.GenerateFilterCondition("AmountUSD", QueryComparisons.Equal, transactionAmountUSD.ToString());
            string filter4 = TableQuery.GenerateFilterCondition("AmountCrypto", QueryComparisons.Equal, transactionAmountCrypto.ToString());
            string filter5 = TableQuery.GenerateFilterCondition("DateAndTime", QueryComparisons.Equal, transactionDateAndTime.ToString());
            string filter6 = TableQuery.GenerateFilterCondition("UserEmail", QueryComparisons.Equal, userEmail);

            string combinedFilter = TableQuery.CombineFilters(
                TableQuery.CombineFilters(
                    TableQuery.CombineFilters(filter1, TableOperators.And, filter2),
                    TableOperators.And,
                    TableQuery.CombineFilters(filter3, TableOperators.And, filter4)
                ),
                TableOperators.And,
                TableQuery.CombineFilters(filter5, TableOperators.And, filter6)
            );

            TableQuery<Transaction> query = new TableQuery<Transaction>().Where(combinedFilter);
            Transaction transactionToDelete = _table.ExecuteQuery(query).ToList()[0];

            TableOperation operation = TableOperation.Delete(transactionToDelete);
            _table.Execute(operation);
        }
    }   
}
