﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetFinderDAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetFinderDAL.Context
{
   public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
           : base(options)
        {
        }

        public DbSet<Pet> Pets { get; set; }
        public DbSet<Shelter> Shelters { get; set; }
        public DbSet<Location> Locations{ get; set; }
        public DbSet<PetColor> PetColors{ get; set; }
        public DbSet<PetRace> PetRaces{ get; set; }
        public DbSet<PetPicture> PetPictures{ get; set; }
        public DbSet<PetKind> PetKind { get; set; }
        public DbSet<FavoriteList> FavoriteList { get; set; }
        public DbSet<AppointmentStatus> AppointmentStatuses { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Seed();
        }
    }
}
