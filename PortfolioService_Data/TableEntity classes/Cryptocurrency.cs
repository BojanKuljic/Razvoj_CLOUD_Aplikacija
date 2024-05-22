using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioServiceStorage.TableEntityClasses {
    public class Cryptocurrency : TableEntity {
        public Cryptocurrency(string name, double amount, string userEmail) {
            this.PartitionKey = "CryptocurrencyPartition";
            this.RowKey = Guid.NewGuid().ToString();
            this.Name = name;
            this.Amount = 0;
            this.ProfitOrLoss = 0;
            this.UserEmail = userEmail;
        }

        public string Name { get; set; }
        public double Amount { get; set; }
        public double ProfitOrLoss { get; set; }
        public string UserEmail { get; set; }
    }
}
