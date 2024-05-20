using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PortfolioService.Models
{
    public class RegisterViewModel
    {
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        
        [Display(Name = "Last name")]
        public string LastName { get; set; }
        
        [Display(Name = "Address")]
        public string Address { get; set; }
        
        [Display(Name = "City")]
        public string City { get; set; }
        
        [Display(Name = "Country")]
        public string Country { get; set; }
        
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
        
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Profile picture")] 
        public HttpPostedFileBase ProfilePicture { get; set; }
    }
}