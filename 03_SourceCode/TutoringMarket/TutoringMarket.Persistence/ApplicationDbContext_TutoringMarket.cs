using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoringMarket.Core.Enities;

namespace TutoringMarket.Persistence
{
    public class ApplicationDbContext_TutoringMarket : DbContext
    {
        public DbSet<Tutor> Tutors { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<SchoolClass> SchoolClasses { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<TeacherComment> TeacherComments { get; set; }
        public DbSet<TutorRequest> TutorRequests { get; set; }

        public ApplicationDbContext_TutoringMarket() : base("name=DefaultConnection")
        {

        }

        public ApplicationDbContext_TutoringMarket(string nameOrConnectionString) : base(nameOrConnectionString)
        {

        }
    }
}
