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
        public virtual string FirstName { get; set; }
      
        public virtual string LastName { get; set; }
       
        public virtual string Email { get; set; }
    
        public virtual string Password { get; set; }
     
        public virtual string ConfirmPassword { get; set; }
   
        public virtual string Street { get; set; }
    
        public virtual int HouseNumber { get; set; }
   
        public virtual string PhoneNumber { get; set; }

        public virtual string City { get; set; }

        public virtual string Zipcode { get; set; }
 
        public virtual string Country { get; set; }

        public virtual string Description { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

    }
}
