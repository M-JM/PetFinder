using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetFinderDAL.Models
{
    public class ApplicationUser : IdentityUser
    {
    
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [ForeignKey("Location")]
        public int? LocationId { get; set; }

        public virtual Location Location { get; set; }

        [ForeignKey("Shelter")]
        public int? ShelterId { get; set; }

        public virtual Shelter Shelter { get; set; }

       
    }
}
