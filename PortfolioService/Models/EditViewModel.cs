using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PortfolioService.Models {
    public class EditViewModel {


        [Required(ErrorMessage = "First name is required.")]
        [StringLength(20, ErrorMessage = "First name cannot be longer than 20 characters.")]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(20, ErrorMessage = "First name cannot be longer than 20 characters.")]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [Display(Name = "Email adress")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(30, ErrorMessage = "Address cannot be longer than 30 characters.")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(20, ErrorMessage = "First name cannot be longer than 20 characters.")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Profile picture")]
        public HttpPostedFileBase ProfilePicture { get; set; }

        [StringLength(100, MinimumLength = 3, ErrorMessage = "Password must be at least 3 characters long.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}