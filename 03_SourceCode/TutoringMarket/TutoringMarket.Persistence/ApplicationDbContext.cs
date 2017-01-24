using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoringMarket.Core.Enities;

namespace TutoringMarket.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Tutor> tutors { get; set; }

        public ApplicationDbContext() : base("name=DefaultConnection")
        {

        }

        public ApplicationDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {

        }
    }
}
