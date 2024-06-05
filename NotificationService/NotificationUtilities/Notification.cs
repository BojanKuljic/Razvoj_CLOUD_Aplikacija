using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.NotificationUtilities
{
    public class Notification : TableEntity
    {
        public Notification(string dateAndTime, string alarm)
        {
            this.PartitionKey = "NotificationPartition";
            this.RowKey = Guid.NewGuid().ToString();
            this.DateAndTime = dateAndTime;
            this.Alarm = alarm;
        }

        public Notification() { }

        public string DateAndTime { get; set; }
        public string Alarm { get; set; }
    }
}
