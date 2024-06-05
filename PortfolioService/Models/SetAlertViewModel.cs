using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PortfolioService.Models
{
    public class SetAlertViewModel
    {
        [Required(ErrorMessage = "Cryptocurrency name is required.")]
        [Display(Name = "Cryptocurrency type: ")]
        public string CryptocurrencyName { get; set; }

        [Required(ErrorMessage = "Alert threshold is required.")]
        [Display(Name = "Alert threshold: ")]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid threshold.")]
        public double AlertThreshold { get; set; }

        [Display(Name = "Lower than threshold: ")]
        public bool IsLowerTreshold { get; set; }
    }
}