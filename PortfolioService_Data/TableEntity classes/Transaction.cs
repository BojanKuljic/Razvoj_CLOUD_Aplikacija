using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioServiceStorage.TableEntityClasses {
    public class Transaction : TableEntity {
        public Transaction(string cryptocurrencyName, string type, double amountUSD, double amountCrypto, string dateAndTime, string userEmail) {
            this.PartitionKey = "TransactionPartition";
            this.RowKey = Guid.NewGuid().ToString();
            this.CryptocurrencyName = cryptocurrencyName;
            this.Type = type;
            this.AmountUSD = amountUSD;
            this.AmountCrypto = amountCrypto;
            this.DateAndTime = dateAndTime;
            this.UserEmail = userEmail;
        }

        public Transaction() { }

        public string CryptocurrencyName { get; set; }
        public string Type { get; set; }
        public double AmountUSD { get; set; }
        public double AmountCrypto { get; set; }
        public string DateAndTime { get; set; }  // It's not DateTime because it would be converted to UTC time zone
        public string UserEmail { get; set; }
    }
}
