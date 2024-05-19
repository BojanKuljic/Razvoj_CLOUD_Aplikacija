using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace PortfolioService_Data
{
    public class CryptocurrencyTransaction : TableEntity
    {
        public CryptocurrencyTransaction(string email)
        {
            this.PartitionKey = "CryptoTransactionPartition"; 
            this.RowKey = email;  
        }

        public CryptocurrencyTransaction() { }

        public enum TransactionType { purchase, sell };

        public string Name { get; set; }
        public double Amount { get; set; }
        public TransactionType Type { get; set; }
        public double PurchaseOrSellValueInUSD { get; set; }
        public DateTime TransactionDate { get; set; }
    }

}
