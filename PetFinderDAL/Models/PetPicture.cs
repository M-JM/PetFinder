using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PetFinderDAL.Models
{
   public class PetPicture
    {

        public int PetPictureId { get; set; }

        public string PhotoPath { get; set; }

        [ForeignKey("Pet")]

        public int PetId { get; set; }
        public virtual Pet Pet{ get; set; }

    }
}
