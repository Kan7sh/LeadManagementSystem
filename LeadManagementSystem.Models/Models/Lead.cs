using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LeadManagementSystem.Models
{
    public class Lead
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is Required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number is Required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid Phone Number. It must be 10 digits.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Address is Required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is Required")]
        public string City { get; set; }

        [Required(ErrorMessage = "Source is Required")]

        public string Source { get; set; }


        [Required(ErrorMessage = "Agent is Required")]
        public string AgentId {  get; set; }

        public string? AgentName { get; set; }

        public string? Status { get; set; }



    }
}
