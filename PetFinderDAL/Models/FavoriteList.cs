using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PetFinderDAL.Models
{
   public class FavoriteList
    {
        public int FavoritelistId { get; set; }

        [ForeignKey("Pet")]
        public int PetId { get; set; }

        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }

        public virtual Pet Pet { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
