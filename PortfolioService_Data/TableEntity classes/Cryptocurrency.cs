using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioServiceStorage.TableEntityClasses {
    public class Cryptocurrency : TableEntity {
        public Cryptocurrency(string name, double initialAmount, double initialAmountUSD, string userEmail, bool isSelected = false) {
            this.PartitionKey = "CryptocurrencyPartition";
            this.RowKey = Guid.NewGuid().ToString();
            this.Name = name;
            this.Amount = initialAmount;
            this.ProfitOrLossUSD = -initialAmountUSD;
            this.UserEmail = userEmail;
            this.IsSelected = isSelected;
        }

        public Cryptocurrency() { }

        public string Name { get; set; }
        public double Amount { get; set; }
        public double ProfitOrLossUSD { get; set; }
        public string UserEmail { get; set; }
        public bool IsSelected;
    }
}
