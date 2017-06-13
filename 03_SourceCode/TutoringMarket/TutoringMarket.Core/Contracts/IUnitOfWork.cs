using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoringMarket.Core.Enities;

namespace TutoringMarket.Core.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        
        IGenericRepository<Tutor> TutorRepository { get; }
        IGenericRepository<SchoolClass> ClassRepository { get; }
        IGenericRepository<Department> DepartmentRepository { get; }
        IGenericRepository<Review> ReviewRepository { get; }
        IGenericRepository<Subject> SubjectRepository { get; }
        IGenericRepository<Tutor_Subject> TutorSubjectRepository { get; }


        void Save();

        void DeleteDatabase();
        List<Tutor> GetTutorsByReviews();
    }
}
