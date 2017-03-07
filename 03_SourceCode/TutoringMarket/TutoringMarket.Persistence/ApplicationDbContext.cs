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
        //TODO maybe add further DBSets
        public DbSet<Tutor> Tutors { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Tutor_Subject> TutorSubjects { get; set; }


        public ApplicationDbContext() : base("name=DefaultConnection")
        {

        }

        public ApplicationDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {

        }
    }
}
