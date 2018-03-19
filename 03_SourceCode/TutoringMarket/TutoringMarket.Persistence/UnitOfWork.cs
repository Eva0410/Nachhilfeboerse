﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoringMarket.Core.Contracts;
using TutoringMarket.Core.Enities;
using TutoringMarket.Core.Statistics;

namespace TutoringMarket.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext_TutoringMarket _context; //= new ApplicationDbContext_TutoringMarket();
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
        private GenericRepository<TeacherComment> _teacherCommentRepository;

        public IGenericRepository<TeacherComment> TeacherCommentRepository
        {
            get
            {
                if (_teacherCommentRepository == null)
                    _teacherCommentRepository = new GenericRepository<TeacherComment>(_context);
                return _teacherCommentRepository;
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

        /// <summary>
        /// Liefert sortierte Liste von Tutoren nach Reviews zurück
        /// </summary>
        /// <param name="disposing"></param>
        public List<Tutor> GetTutorsByReviews()
        {
            return _context.Reviews.GroupBy(r => r.Tutor).OrderByDescending(grp => grp.Average(r => r.Books)).Select(grp => grp.Key).ToList();
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

        public string GetDepartmentWithMostTutors()
        {
            return _context.Tutors.Include(nameof(Tutor.Department)).Where(t => t.Accepted).GroupBy(t => t.Department).OrderByDescending(grp => grp.Count()).FirstOrDefault().Key.Name;
        }

        public int GetTutorsCount()
        {
            return _context.Tutors.Count(t => t.Accepted);
        }

        public int GetTutorsWithImagePercentage()
        {
            try
            {
                return _context.Tutors.Count(t => !String.IsNullOrEmpty(t.Image) && t.Accepted) / _context.Tutors.Count(t => t.Accepted);
            }
            catch(Exception e)
            {
                return 0;
            }
        }

        List<DataPoint> IUnitOfWork.GetTutorsPerGender()
        {
            var list = new List<DataPoint>();
            list.Add(new DataPoint() { Label = "Weiblich", Y = _context.Tutors.Count(t => t.Gender == "Weiblich" && t.Accepted) });
            list.Add(new DataPoint() { Label="Männlich", Y=_context.Tutors.Count(t => t.Gender == "Männlich" && t.Accepted) });
            return list;
        }

        List<DataPoint> IUnitOfWork.GetTutorsPerClass()
        {
           return _context.Tutors.Where(t => t.Accepted).GroupBy(t => t.Class).Select(grp =>
            new DataPoint()
            {
                Y = grp.Count(),
                Label = grp.Key.Name
            }).ToList();
        }
    }
}
