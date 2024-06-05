using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using NotificationService.NotificationUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService
{
    public class NotificationRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;

        public NotificationRepository()
        {
            _storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("NotificationTable");
            _table.CreateIfNotExists();
        }

        public void InsertNotification(Notification notification)
        {
            Loop:
            try
            {
                TableOperation insertOperation = TableOperation.Insert(notification);
                _table.Execute(insertOperation);
            }
            catch
            {
                notification.RowKey = Guid.NewGuid().ToString();
                goto Loop;
            }
        }

    }
}
