using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.ViewModels.AccountViewModel
{
    public abstract class RegisterViewModel
    {
        public virtual string Name { get; set; }
        [Required]
        public virtual string FirstName { get; set; }
        [Required]
        public virtual string LastName { get; set; }
        [Required]
        public virtual string Email { get; set; }
        [Required]
        public virtual string Password { get; set; }
        [Required]
        public virtual string ConfirmPassword { get; set; }
        [Required]
        public virtual string Street { get; set; }
        [Required]
        public virtual int HouseNumber { get; set; }
        [Required(ErrorMessage = "You must provide a phone number")]
        public virtual string PhoneNumber { get; set; }
        [Required]
        public virtual string City { get; set; }
        [Required]
        public virtual string Zipcode { get; set; }
        [Required]
        public virtual string Country { get; set; }

        public virtual string Description { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

    }
}
