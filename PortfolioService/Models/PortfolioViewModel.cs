using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortfolioService.Models
{
    public class PortfolioViewModel
    {
        public List<CryptocurrencyHolding> Holdings { get; set; }
        public double TotalValue { get; set; }
        public double TotalProfitOrLoss { get; set; }
    }

    public class CryptocurrencyHolding
    {
        public string Name { get; set; }
        public double Amount { get; set; }
        public double Value { get; set; }
        public double ProfitOrLoss { get; set; }
    }
}