using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortfolioService.Models
{
    public class PortfolioViewModel
    {
        public List<CryptocurrencyHolding> Holdings { get; set; }
        public decimal TotalValue { get; set; }
        public decimal TotalProfitOrLoss { get; set; }
    }

    public class CryptocurrencyHolding
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public decimal Value { get; set; }
        public decimal ProfitOrLoss { get; set; }
    }
}