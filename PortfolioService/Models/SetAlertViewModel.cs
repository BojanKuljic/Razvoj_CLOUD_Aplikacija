using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortfolioService.Models
{
    public class SetAlertViewModel
    {
        public string CryptocurrencyName { get; set; }
        public double AlertThreshold { get; set; }

        public bool isLowerTreshold { get; set; }
        public string Email { get; set; }
    }
}