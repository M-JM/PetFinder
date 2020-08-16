using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.ViewModels.AccountViewModel
{
    public class ShelterProfileViewModel
    {

        public int  Id { get; set; }

        [EmailAddress]
        public string Email { get; set; }
  
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public int HouseNumber { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Zipcode { get; set; }
        [Required]
        public string Country { get; set; }

        public string Description { get; set; }
    }
}
