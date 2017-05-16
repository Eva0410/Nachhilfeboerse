using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoringMarket.Core.Contracts;
using TutoringMarket.Core.Enities;

namespace TutoringMarket.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext_TutoringMarket _context = new ApplicationDbContext_TutoringMarket();
        private bool _disposed;

        /// <summary>
        ///     Konkrete Repositories. Keine Ableitung erforderlich
        /// </summary>
        private GenericRepository<Tutor> _tutorRepository;

        public IGenericRepository<Tutor> TutorRepository
        {
            get
            {
                if (_tutorRepository == null)
                    _tutorRepository = new GenericRepository<Tutor>(_context);
                return _tutorRepository;
            }
        }
        private GenericRepository<SchoolClass> _classRepository;

        public IGenericRepository<SchoolClass> ClassRepository
        {
            get
            {
                if (_classRepository == null)
                    _classRepository = new GenericRepository<SchoolClass>(_context);
                return _classRepository;
            }
        }
        private GenericRepository<Department> _departmentRepository;

        public IGenericRepository<Department> DepartmentRepository
        {
            get
            {
                if (_departmentRepository == null)
                    _departmentRepository = new GenericRepository<Department>(_context);
                return _departmentRepository;
            }
        }
        private GenericRepository<Review> _reviewRepository;

        public IGenericRepository<Review> ReviewRepository
        {
            get
            {
                if (_reviewRepository == null)
                    _reviewRepository = new GenericRepository<Review>(_context);
                return _reviewRepository;
            }
        }
        private GenericRepository<Subject> _subjectRepository;

        public IGenericRepository<Subject> SubjectRepository
        {
            get
            {
                if (_subjectRepository == null)
                    _subjectRepository = new GenericRepository<Subject>(_context);
                return _subjectRepository;
            }
        }
        private GenericRepository<Tutor_Subject> _tutorSubjectRepository;

        public IGenericRepository<Tutor_Subject> TutorSubjectRepository
        {
            get
            {
                if (_tutorSubjectRepository == null)
                    _tutorSubjectRepository = new GenericRepository<Tutor_Subject>(_context);
                return _tutorSubjectRepository;
            }
        }


        public UnitOfWork(string connectionString)
        {
            _context = new ApplicationDbContext_TutoringMarket(connectionString);
        }

        public UnitOfWork() : this("name=DefaultConnection")
        {

        }

        /// <summary>
        ///     Repository-übergreifendes Speichern der Änderungen
        /// </summary>
        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void DeleteDatabase()
        {
            _context.Database.Delete();
        }

    }
}
