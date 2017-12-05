using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TutoringMarket.WebIdentity.Data;
using TutoringMarket.WebIdentity.Models;
using TutoringMarket.WebIdentity.Services;
using TutoringMarket.Core.Contracts;
using TutoringMarket.Persistence;
using Microsoft.AspNetCore.Identity;

namespace TutoringMarket.WebIdentity
{
    public class Startup
    {

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
           //     builder.AddUserSecrets();

                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultIdentityConnection")));

            services.AddTransient<IUnitOfWork, UnitOfWork>(p => new UnitOfWork(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        public async void InitRolesAndUsers(IServiceProvider provider)
        {
            var RoleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
            IUnitOfWork uow = provider.GetRequiredService<IUnitOfWork>();

            await CreateRole(RoleManager, "Admin");
            await CreateRole(RoleManager, "Tutor");
            await CreateRole(RoleManager, "Visitor");
            await CreateRole(RoleManager, "Teacher");

            await UserManager.CreateAsync(new ApplicationUser { UserName = "in130021", FirstName="Eva", LastName="Pürmayr", SchoolClass="5AHIF", Department="Informatik" });
            await UserManager.CreateAsync(new ApplicationUser { UserName = "in130019", FirstName="Danijal", LastName="Orascanin", SchoolClass="5AHIF", Department="Informatik"});
            await UserManager.CreateAsync(new ApplicationUser { UserName = "a.keck", FirstName = "a", LastName = "keck"});

            await AddUserRole("in130021", "Admin", UserManager, RoleManager);
            await AddUserRole("in130019", "Admin", UserManager, RoleManager);
            await AddUserRole("a.keck", "Admin", UserManager, RoleManager);
            await AddUserRole("in130021", "Visitor", UserManager, RoleManager);
            await AddUserRole("in130019", "Visitor", UserManager, RoleManager);
            await AddUserRole("a.keck", "Visitor", UserManager, RoleManager);
            await AddUserRole("a.keck", "Teacher", UserManager, RoleManager);

            //Tutoren adden
            //await UserManager.CreateAsync(new ApplicationUser { UserName = "in130024", FirstName = "Jakob", LastName = "Rumpelsberger", SchoolClass = "5AHIF", Department = "Informatik" });
            //await UserManager.CreateAsync(new ApplicationUser { UserName = "in130015", FirstName = "Michael", LastName = "Medweschek", SchoolClass = "5AHIF", Department = "Informatik" });


            //Rollen für Test-Tutoren
            //TODO Test Tutoren löschen
            //await AddUserRole("in130021", "Tutor", UserManager, RoleManager);
            //await AddUserRole("in130021", "Teacher", UserManager, RoleManager);
            //await AddUserRole("in130019", "Teacher", UserManager, RoleManager);
            //await AddUserRole("in130024", "Tutor", UserManager, RoleManager);
            //await AddUserRole("in130015", "Tutor", UserManager, RoleManager);

        }
        private async Task CreateRole(RoleManager<IdentityRole> rm, string roleName)
        {
            IdentityResult roleResult = null;

            if (!await rm.RoleExistsAsync(roleName))
            {
                roleResult = await rm.CreateAsync(new IdentityRole(roleName));
            }
            if (roleResult == null || !roleResult.Succeeded)
            {
                Console.WriteLine("Debug-Info: Rolle " + roleName + " konnte nicht erstellt werden! Timestamp: " + DateTime.Now);
            }
        }
        private async Task AddUserRole(string name, string roleName, UserManager<ApplicationUser> um, RoleManager<IdentityRole> rm)
        {
            var user = await um.FindByNameAsync(name);
            var role = await rm.FindByNameAsync(roleName);
            var inRole = await um.IsInRoleAsync(user, roleName);
            IdentityResult result = null;

            if (user != null && !inRole)
            {
                result = await um.AddToRoleAsync(user, roleName);
            }
            if (result == null || !result.Succeeded)
            {
                Console.WriteLine("Debug-Info: User " + name + "konnte nicht zur Rolle " + roleName + " hinzugefügt werden! Timestamp: " + DateTime.Now);
            }
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider provider)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Account}/{action=Login}/{id?}");
            });
            InitRolesAndUsers(provider);
        }
    }
}
