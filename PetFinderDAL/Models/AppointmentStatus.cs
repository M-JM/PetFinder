using System;
using System.Collections.Generic;
using System.Text;

namespace PetFinderDAL.Models
{
    public class AppointmentStatus
    {
        public int AppointmentStatusId { get; set; }

        public string StatusName { get; set; }

        public string Color { get; set; }
    }
}
