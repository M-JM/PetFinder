﻿using PetFinderDAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PetFinderDAL.Repositories
{
  public  interface IAppointmentRepository 
    {
        Appointment AddAppointment(Appointment appointment);
        Appointment UpdateAppointment(Appointment appointment);
        List<Appointment> GetAppointments(int? shelterId);

        List<Appointment> GetAppointmentsUsers(string userId);

        Appointment GetAppointment(int AppointmentId);

        List<Appointment> GetHoursofAppointment(int PetId, DateTime Time);

        List<AppointmentStatus> GetStatus();
    }
}
