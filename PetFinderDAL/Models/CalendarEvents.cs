using System;
using System.Collections.Generic;
using System.Text;

namespace PetFinderDAL.Models
{
    public class CalendarEvents
    {
        public int AppointmentId { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public string Title { get; set; }

        public string User { get; set; }

        public string BackgroundColor { get; set; }

        public Boolean allDay { get; set; }

        public string Status { get; set; }

        public int appointmentstatusId { get; set; }
    }
}
