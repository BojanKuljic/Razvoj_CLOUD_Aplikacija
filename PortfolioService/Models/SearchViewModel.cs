using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortfolioService.Models
{
    public class SearchViewModel
    {
        public string SearchQuery { get; set; }
        public List<Cryptocurrency> Results { get; set; }
    }

    public class Cryptocurrency
    {
        public string Name { get; set; }
        public decimal CurrentPrice { get; set; }
    }
}