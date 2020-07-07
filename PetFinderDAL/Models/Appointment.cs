using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PetFinderDAL.Models
{
   public class Appointment
    {
        public int AppointmentId { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        [ForeignKey("Shelter")]

        public int? ShelterId { get; set; }
        public virtual Shelter Shelter { get; set; }

        [ForeignKey("AppointmentStatus")]

        public int AppointmentStatusId { get; set; }
        public virtual AppointmentStatus AppointmentStatus { get; set; }

        [ForeignKey("Pet")]

        public int PetId { get; set; }
        public virtual Pet Pet{ get; set; }

        [ForeignKey("ApplicationUser")]

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
