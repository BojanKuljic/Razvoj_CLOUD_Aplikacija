using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure;
using System.Web;
using Microsoft.Azure;

namespace PortfolioServiceStorage
{
    public class UserRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;

        public UserRepository()
        {
            _storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true"); //var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("UserTable"); 
            _table.CreateIfNotExists();
        }

        public void InsertOrMergeUser(User user)
        {
            user.RowKey = user.Email;
            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(user);
            _table.Execute(insertOrMergeOperation);
        }

        public User RetrieveUser(string email)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<User>("UserPartition", email);
            TableResult result = _table.Execute(retrieveOperation);
            return result.Result as User;
        }

        public void DeleteUser(User user)
        {
            TableOperation operation = TableOperation.Delete(user);
            _table.Execute(operation);
        }
    }
}
