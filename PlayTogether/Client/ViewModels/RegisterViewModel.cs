using PlayTogether.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlayTogether.Client.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "First Name")]
        [RegularExpression("^([A-Z][a-zA-Z]{1,})$", ErrorMessage = "Please enter a valid first name.")]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Last Name")]
        [RegularExpression("^([A-Z][a-zA-Z]{1,})$", ErrorMessage = "Please enter a valid last name.")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Username")]
        [RegularExpression("^(?=[a-zA-Z0-9._]{6,20}$)(?!.*[_.]{2})[^_.].*[^_.]$", ErrorMessage = "Please enter a valid username.")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please enter your Date of Birth.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Please select your Country of Residence.")]
        [Display(Name = "Country of Residence")]
        public int? CountryOfResidenceId { get; set; }

        [Required(ErrorMessage = "Please select your Gender.")]
        [Display(Name = "Gender")]
        public int? GenderId { get; set; }
    }
}
