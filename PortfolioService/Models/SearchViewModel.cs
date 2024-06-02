using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PortfolioService.Models
{
    public class SearchViewModel
    {
        [Required(ErrorMessage = "Cryptocurrency type is a required field!")]
        [Display(Name = "Cryptocurrency: ")]
        public string CryptocurrencyType { get; set; }
    }
}