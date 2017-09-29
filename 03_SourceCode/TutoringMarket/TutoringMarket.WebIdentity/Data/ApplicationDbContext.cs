using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TutoringMarket.WebIdentity.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace TutoringMarket.WebIdentity.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public ApplicationDbContext()
            : base()
        {
        }

        public ApplicationDbContext Create(DbContextFactoryOptions options)
        {

            //   Used only for EF.NET Core CLI tools(update database / migrations etc.)

            var builder = new ConfigurationBuilder().AddJsonFile(Directory.GetCurrentDirectory() + "\\appsettings.json", optional: false, reloadOnChange: true);

            var config = builder.Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(config.GetConnectionString("DefaultIdentityConnection"));

            return new ApplicationDbContext(optionsBuilder.Options);
        }


    }
}
