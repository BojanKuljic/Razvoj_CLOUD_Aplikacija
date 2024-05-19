using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace PortfolioService_Data
{
    public class CryptocurrencyDataRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;

        public CryptocurrencyDataRepository()
        {
            _storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true"); //var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("UserTable"); _table.CreateIfNotExists();
        }

        public void Initialize()
        {
            _table.CreateIfNotExists();
        }

        public void InsertOrMergeCryptocurrency(CryptocurrencyTransaction crypto, string email)
        {
            crypto.RowKey = email;
            TableOperation operation = TableOperation.InsertOrMerge(crypto);
            _table.Execute(operation);
        }

        public CryptocurrencyTransaction RetrieveCryptocurrency(string email)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<CryptocurrencyTransaction>("CryptoTransactionPartition", email);
            TableResult result = _table.Execute(retrieveOperation);
            return result.Result as CryptocurrencyTransaction;
        }

        public void DeleteCryptocurrency(CryptocurrencyTransaction crypto)
        {
            TableOperation operation = TableOperation.Delete(crypto);
            _table.Execute(operation);
        }
    }

}
