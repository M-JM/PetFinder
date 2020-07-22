using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using PetFinderDAL.Context;
using PetFinderDAL.Models;
using PetFinderDAL.Repositories;


namespace PetFinder
{
    public class Startup
    {

        private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
          
            services.AddDbContextPool<AppDbContext>(
              options =>
                  options.UseSqlServer(_config.GetConnectionString("PetDBConnection"))
          );
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 10;
                options.Password.RequiredUniqueChars = 3;
            }).AddEntityFrameworkStores<AppDbContext>();
                services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
             });

            services.AddControllersWithViews().AddNewtonsoftJson(
        options => options.SerializerSettings.ReferenceLoopHandling =
        ReferenceLoopHandling.Ignore
    );

            services.AddScoped<IPetRepository, PetRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<IShelterRepository, ShelterRepository>();
            services.AddScoped<IFavoriteRepository, FavoriteRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddHttpClient<Controllers.AccountController>();

        

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
           
            app.UseAuthorization();
            app.UseSession();

            /* Cookies vs Session
             * https://docs.microsoft.com/en-us/archive/msdn-magazine/2003/april/nine-options-for-managing-persistent-user-state-in-asp-net
             ^-- old but nice overview of different techniques to store data acroos WebApp and when to use what
            The concept is storing persistent data across page loads for a web visitor.
            Cookies store it directly on the client.Sessions use a cookie as a key of sorts, to associate with the data that is stored on the server side.
            It is preferred to use sessions because the actual values are hidden from the client, --> disputed information
            Sessions use cookies (see below).
            Sessions are safer than cookies, but not invulnarable.
            Expiration is reset when the user refreshes or loads a new page.

            and you control when the data expires and becomes invalid. --> disputed information
            If it was all based on cookies, a user(or hacker) could manipulate their cookie data and then play requests to your site.
            */
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
