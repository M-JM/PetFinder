using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PetFinderDAL.Models
{
    public class Location
    {
        [Key]
        public int LocationtId { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public int HouseNumber { get; set; }

        public float Latitude { get; set; }

        public float Longitude { get; set; }

        public virtual Shelter Shelter { get; set; }

        public virtual ApplicationUser ApplicationUser{ get; set; }

    }
}
