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
        public string CryptocurrencyName { get; set; }

        [Required(ErrorMessage = "Alert threshold is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid threshold.")]
        public double AlertThreshold { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        public bool IsLowerTreshold { get; set; }

    }
}