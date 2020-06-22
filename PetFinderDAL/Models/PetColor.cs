using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PetFinderDAL.Models
{
   public class PetColor
    {
        public int PetColorId { get; set; }

        public string Color { get; set; }

        public List<Pet> Pets { get; set; }

    }
}
