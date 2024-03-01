using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace LeadManagementSystem.Models
{
    public class Manager 
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Name is Required")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number is Required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid Phone Number. It must be 10 digits.")]
        public  string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        [StringLength(255, ErrorMessage = "Password Must be between greater than 8 characters", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Password must contain Alpha Numeric values, Special Character and Uppercase character")]
        public string PasswordHash { get; set; }

        [Required(ErrorMessage = "Address is Required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is Required")]
        public string City { get; set; }

    }
}



