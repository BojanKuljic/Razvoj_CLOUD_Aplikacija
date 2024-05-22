using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioServiceStorage.TableEntityClasses {
    public enum TransactionType { Buy, Sell }
    public class Transaction : TableEntity {
        public Transaction(string cryptocurrencyName, TransactionType type, double amount, DateTime dateAndTime, string userEmail) {
            this.PartitionKey = "TransactionPartition";
            this.RowKey = Guid.NewGuid().ToString();
            this.CryptocurrencyName = cryptocurrencyName;
            this.Type = type;
            this.Amount = amount;
            this.DateAndTime = dateAndTime;
            this.UserEmail = userEmail;
        }

        public string CryptocurrencyName { get; set; }
        public TransactionType Type { get; set; }
        public double Amount { get; set; }
        public DateTime DateAndTime { get; set; }
        public string UserEmail { get; set; }
    }
}
