using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PetFinderDAL.Models
{
  public class ApplicationUser : IdentityUser
    {
        [ForeignKey("Location")]
        public int? LocationId { get; set; }

        public virtual Location Location { get; set; }

        [ForeignKey("Shelter")]
        public int? ShelterId { get; set; }

        public virtual Shelter Shelter { get; set; }


        [ForeignKey("FavoriteList")]
        public int? FavoritelistId { get; set; }

        public virtual FavoriteList FavoriteList { get; set; }

    }
}
