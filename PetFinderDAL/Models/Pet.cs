using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PetFinderDAL.Models
{
    public class Pet
    {
        public int PetId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Gender { get; set; }

        public  DateTime DOB { get; set; }

        public string Size { get; set; }
        [Required]
        public string Description { get; set; }

        public string Social { get; set; }
             
             
        //Social with kids,dogs,cats stored as comma separated String.
        //When retrieving the value to display , split(",") to get the values as Array.

        public List<PetPicture> PetPictures { get; set; }

        [ForeignKey("Shelter")]

        public int ShelterId { get; set; }
        public virtual Shelter Shelter { get; set; }

        [ForeignKey("PetColor")]
        public int PetColorId { get; set; }
        public virtual PetColor PetColor { get; set; }

        [ForeignKey("PetRace")]
        public int PetRaceId { get; set; }

        public virtual PetRace PetRace { get; set; }

        [ForeignKey("PetKind")]
        public int PetKindId { get; set; }

        public virtual PetKind PetKind { get; set; }

    }
}
