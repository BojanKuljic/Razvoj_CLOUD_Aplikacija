using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortfolioServiceStorage.TableEntityClasses;
using System.Web.UI.WebControls;
using System.Xml;

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

        public IEnumerable<Cryptocurrency> ReadUsersCryptocurrencies(string userEmail) {
            string filter = TableQuery.GenerateFilterCondition("UserEmail", QueryComparisons.Equal, userEmail);

            TableQuery<Cryptocurrency> query = new TableQuery<Cryptocurrency>().Where(filter);

            return _table.ExecuteQuery(query);
        }

        public bool UpdateAmountAndProfitOrLoss(string userEmail, string cryptocurrencyName, string transactionType, double transactionAmountUSD, double convertedTransactionAmount) {
            string emailFilter = TableQuery.GenerateFilterCondition("UserEmail", QueryComparisons.Equal, userEmail);
            string nameFilter = TableQuery.GenerateFilterCondition("Name", QueryComparisons.Equal, cryptocurrencyName);
            string combinedFilter = TableQuery.CombineFilters(emailFilter, TableOperators.And, nameFilter);

            TableQuery<Cryptocurrency> query = new TableQuery<Cryptocurrency>().Where(combinedFilter);
            var result = _table.ExecuteQuery(query).ToList();

            if (result.Count == 0) {
                if (transactionType == "Sale") {
                    return false;  // Can't sell if they don't have it 
                }

                Create(new Cryptocurrency(cryptocurrencyName, convertedTransactionAmount, transactionAmountUSD, userEmail));
            } else {
                Cryptocurrency cryptocurrency = result[0];

                if (transactionType == "Purchase") {
                    cryptocurrency.Amount += convertedTransactionAmount;
                    cryptocurrency.ProfitOrLossUSD -= transactionAmountUSD;
                } else {
                    if (cryptocurrency.Amount < convertedTransactionAmount) {
                        return false;  // Can't sell if they don't have enough of it 
                    }

                    cryptocurrency.Amount -= convertedTransactionAmount;
                    cryptocurrency.ProfitOrLossUSD += transactionAmountUSD;
                }

                TableOperation replaceOperation = TableOperation.Replace(cryptocurrency);
                _table.Execute(replaceOperation);
            }

            return true;
        }

        public void Delete(string cryptocurrencyName, string userEmail) {
            string emailFilter = TableQuery.GenerateFilterCondition("UserEmail", QueryComparisons.Equal, userEmail);
            string nameFilter = TableQuery.GenerateFilterCondition("Name", QueryComparisons.Equal, cryptocurrencyName);
            string combinedFilter = TableQuery.CombineFilters(emailFilter, TableOperators.And, nameFilter);

            TableQuery<Cryptocurrency> query = new TableQuery<Cryptocurrency>().Where(combinedFilter);
            Cryptocurrency cryptocurrencyToDelete = _table.ExecuteQuery(query).ToList()[0];

            TableOperation operation = TableOperation.Delete(cryptocurrencyToDelete);
            _table.Execute(operation);
        }
    }
}
