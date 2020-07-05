using System;
using System.Collections.Generic;
using System.Text;

namespace PetFinderDAL.Models
{
   public class FavoriteList
    {
        public int? FavoritelistId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
