using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PetFinderDAL.Models
{
    public enum Tristate { 
    Yes,
    No,
    Unknown
    }

    public class Pet
    {
        public int PetId { get; set; }
        [Required(ErrorMessage = "Give your pet a name")]
        [DisplayName("Pet Name")]
        [StringLength(30)]
        public string Name { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public  DateTime DOB { get; set; }
        [Required(ErrorMessage = "Please select a value")]
        public string Size { get; set; }
        [Required(ErrorMessage = "Please write a description")]
        [DisplayName("Pet Description")]
        [StringLength(500)]
        public string Description { get; set; }
        [Required(ErrorMessage = "Please select a value")]
        [DisplayName("Social With Dogs")]
        public Tristate? SocialWithDogs { get; set; }
        [Required(ErrorMessage = "Please select a value")]
        [DisplayName("Social With Cats")]
        public Tristate? SocialWithCats { get; set; }
        [Required(ErrorMessage = "Please select a value")]
        [DisplayName("Fit for Appartment")]
        public Tristate? Appartmentfit { get; set; }
        [Required(ErrorMessage = "Please select a value")]
        [DisplayName("Friendly with kids")]
        public Tristate? KidsFriendly { get; set; }

        //Social with kids,dogs,cats stored as comma separated String?
        //When retrieving the value to display , split(",") to get the values as Array?


        public List<PetPicture> PetPictures { get; set; }

        // START HERE WITH FOREIGN KEY AFTER PROPERTIES OF CLASS 

        [ForeignKey("Shelter")]
        public int? ShelterId { get; set; }
        public virtual Shelter Shelter { get; set; }

        [ForeignKey("PetColor")]
        public int PetColorId { get; set; }
        public virtual PetColor PetColor { get; set; }

        [ForeignKey("PetRace")]
        public int PetRaceId { get; set; }

        public virtual PetRace PetRace { get; set; }

        [ForeignKey("PetKind")]
        [DisplayName("Pet Kind")]
        public int PetKindId { get; set; }

        public virtual PetKind PetKind { get; set; }

    }
}
