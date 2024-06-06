using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PortfolioService.Models
{
    public class AddTransactionViewModel
    {
        [Required(ErrorMessage = "Type is a required field!")]
        [Display(Name = "Transaction Type: ")]
        public string TransactionType { get; set; }

        [Required(ErrorMessage = "Cryptocurrency type is a required field!")]
        [Display(Name = "Cryptocurrency: ")]
        public string CryptocurrencyType { get; set; }

        [Required(ErrorMessage = "Amount is a required field!")]
        [Range(5.00, double.MaxValue, ErrorMessage = "Amount must be greater than 5$.")]
        [Display(Name = "Amount: ")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "Date and time is a required field!")]
        [Display(Name = "Date and Time: ")]
        public DateTime DateAndTime { get; set; }
    }
}