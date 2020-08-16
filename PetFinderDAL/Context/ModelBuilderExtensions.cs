using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetFinderDAL.Context
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<IdentityRole>().HasData(


           new IdentityRole()
           {
               Id = "b4e5c024-99c5-43b1-847f-26585777f463",
               Name = "Admin",
               NormalizedName = "ADMIN"
           },
           new IdentityRole()
           {
               Id = "fb4302cf-f521-4fa9-b20a-0d4e59b703a5",
               Name = "User",
               NormalizedName = "USER"

           }
           );

        }
    }
}
