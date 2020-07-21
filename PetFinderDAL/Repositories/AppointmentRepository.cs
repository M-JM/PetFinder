using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PetFinderDAL.Context;
using PetFinderDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PetFinderDAL.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AppointmentRepository> _logger;

        public AppointmentRepository(AppDbContext context, ILogger<AppointmentRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public Appointment AddAppointment(Appointment appointment)
        {
            var newAppointment = _context.Appointments.Add(appointment);

            if (newAppointment != null && newAppointment.State == EntityState.Added)
            {
                var affectedRows = _context.SaveChanges();

                if (affectedRows > 0)
                {
                    return newAppointment.Entity;
                }
            }

            return null;

        }

        public Appointment UpdateAppointment(Appointment appointment)
        {
            var UpdateAppointment = _context.Appointments.Update(appointment);

            if (UpdateAppointment != null && UpdateAppointment.State == EntityState.Modified)
            {
                var affectedRows = _context.SaveChanges();

                if (affectedRows > 0)
                {
                    return UpdateAppointment.Entity;
                }
            }

            return null;

        }

        public List<Appointment> GetAppointments()
        {
           var appointments= _context.Appointments.Include(x => x.Pet).Include(x=>x.AppointmentStatus).Include(x => x.ApplicationUser).ToList();                 

            return appointments;

        }

        public Appointment GetAppointment(int AppointmentId)
        {
            var appointment = _context.Appointments.Where(x=>x.AppointmentId == AppointmentId).FirstOrDefault();

            return appointment;

        }

        public List<AppointmentStatus> GetStatus()
        {
            var statuses = _context.AppointmentStatuses.ToList();

            return statuses;

        }
    }
}
