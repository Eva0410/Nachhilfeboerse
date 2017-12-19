using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using TutoringMarket.WebIdentity.Models.ViewModels;

namespace TutoringMarket.WebIdentity.Models
{
    public class DbModels : DbContext
    {
        public DbModels()
            : base("name=DbModels")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }

        public virtual DbSet<GetTutorModel> Images { get; set; }
    }
}
