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
        CryptocurrencyRepository cryptoRepo;

        public TransactionRepository() {
            _storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true"); //var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("TransactionTable");
            _table.CreateIfNotExists();
            cryptoRepo = new CryptocurrencyRepository();
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

        public void DeleteLastUserTransaction(string userEmail)
        {
            List<Transaction> userTransactions = ReadUsersTransactions(userEmail).ToList();

            if (userTransactions == null || userTransactions.Count == 0)
            {
                Console.WriteLine("No transactions found for this user.");
                return;
            }

            Transaction latestTransaction = null;
            DateTime latestDateTime = DateTime.MinValue;

            foreach(Transaction t in userTransactions)
            {
                DateTime transactionDateTime;

                if(DateTime.TryParse(t.DateAndTime, out transactionDateTime))
                {
                    if(transactionDateTime > latestDateTime)
                    {
                        latestDateTime = transactionDateTime;
                        latestTransaction = t;
                    }
                }
                else
                {
                    Console.WriteLine("Failed to parse DateAndTime");
                }
            }

            if (latestTransaction.Type == "Sale")
            {
                cryptoRepo.UpdateAmountAndProfitOrLoss(userEmail, latestTransaction.CryptocurrencyName, "Purchace", -(latestTransaction.AmountUSD), -latestTransaction.AmountCrypto);
            }
            else
            {
                cryptoRepo.UpdateAmountAndProfitOrLoss(userEmail, latestTransaction.CryptocurrencyName, "Sale", latestTransaction.AmountUSD, latestTransaction.AmountCrypto);
            }

            TableOperation operation = TableOperation.Delete(latestTransaction);
            _table.Execute(operation);
        }

        public void DeleteAllTransactionsForUserCurrency(string cryptocurrencyName, string userEmail)
        {
            string emailFilter = TableQuery.GenerateFilterCondition("UserEmail", QueryComparisons.Equal, userEmail);
            string nameFilter = TableQuery.GenerateFilterCondition("CryptocurrencyName", QueryComparisons.Equal, cryptocurrencyName);
            string combinedFilter = TableQuery.CombineFilters(emailFilter, TableOperators.And, nameFilter);

            TableQuery<Transaction> query = new TableQuery<Transaction>().Where(combinedFilter);
            List<Transaction> transactionsToDelete = _table.ExecuteQuery(query).ToList();

            foreach(Transaction t in transactionsToDelete)
            {
                TableOperation operation = TableOperation.Delete(t);
                _table.Execute(operation);
            }
        }
    }   
}
