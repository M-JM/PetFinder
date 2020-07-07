using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.ViewModels.AccountViewModel
{
    public class UserRegisterViewModel : RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public override string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public override string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password",
            ErrorMessage = "Password and confirmation password do not match.")]
        public override string ConfirmPassword { get; set; }

        [Required]
        public override string Street { get; set; }
        [Required]
        public override int HouseNumber { get; set; }
        [Required]
        public override string City { get; set; }
        [Required]
        public override string Zipcode { get; set; }
        [Required]
        public override string Country { get; set; }
     



    }
}
