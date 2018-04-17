using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoringMarket.Core.Contracts;
using TutoringMarket.Core.Enities;
using TutoringMarket.Core.Statistics;
using Microsoft.AspNetCore.Identity;

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
        private GenericRepository<TutorRequest> _tutorRequestRepository;

        public IGenericRepository<TutorRequest> TutorRequestRepository
        {
            get
            {
                if (_tutorRequestRepository == null)
                    _tutorRequestRepository = new GenericRepository<TutorRequest>(_context);
                return _tutorRequestRepository;
            }
        }
        private GenericRepository<TeacherCommentStatisticEntry> _teacherCommentStatisticsRepository;
        public IGenericRepository<TeacherCommentStatisticEntry> TeacherCommentStatisticsRepository
        {
            get
            {
                if (_teacherCommentStatisticsRepository == null)
                    _teacherCommentStatisticsRepository = new GenericRepository<TeacherCommentStatisticEntry>(_context);
                return _teacherCommentStatisticsRepository;
            }
        }
        private GenericRepository<AcceptStatistics> _acceptStatisticsRespository;
        public IGenericRepository<AcceptStatistics> AcceptStatisticsRepository
        {
            get
            {
                if (_acceptStatisticsRespository == null)
                    _acceptStatisticsRespository = new GenericRepository<AcceptStatistics>(_context);
                return _acceptStatisticsRespository;
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
            try
            {
                return _context.Tutors.Include(nameof(Tutor.Department)).Where(t => t.Accepted).GroupBy(t => t.Department).OrderByDescending(grp => grp.Count()).FirstOrDefault().Key.Name;
            }
            catch(Exception e)
            {
                return "-";
            }
        }

        public int GetTutorsCount()
        {
            try
            {
                return _context.Tutors.Count(t => t.Accepted);
            }
            catch(Exception e)
            {
                return 0;
            }
        }

        public double GetTutorsWithImagePercentage()
        {
            try
            {
                if (GetTutorsCount() == 0)
                    return 0;
                return Math.Round((double)_context.Tutors.Count(t => !String.IsNullOrEmpty(t.Image) && t.Accepted) / (double)GetTutorsCount() * 100, 2);
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        List<DataPoint> IUnitOfWork.GetTutorsPerGender()
        {
            var list = new List<DataPoint>();
            try
            {
                list.Add(new DataPoint() { Label = "Weiblich", Y = _context.Tutors.Count(t => t.Gender == "Weiblich" && t.Accepted) });
                list.Add(new DataPoint() { Label = "Männlich", Y = _context.Tutors.Count(t => t.Gender == "Männlich" && t.Accepted) });
            }
            catch(Exception e)
            {

            }
            return list;
        }

        List<DataPoint> IUnitOfWork.GetTutorsPerClass()
        {
            try
            {
                return _context.Tutors.Where(t => t.Accepted).GroupBy(t => t.Class).Select(grp =>
                 new DataPoint()
                 {
                     Y = grp.Count(),
                     Label = grp.Key.Name
                 }).ToList();
            }
            catch(Exception e)
            {
                return new List<DataPoint>();
            }
        }

        public int GetReviewsCount()
        {
            try
            {
                return _context.Reviews.Count(r => r.Approved);
            }
            catch(Exception e)
            {
                return 0;
            }
        }

        public double GetAverageReview()
        {
            try
            {
                return Math.Round(_context.Reviews.Where(r => r.Approved).Average(r => r.Books), 2);
            }
            catch(Exception e)
            {
                return 0;
            }
        }

        public double GetAverageCountReviewsPerTutor()
        {
            try
            {
                if (GetTutorsCount() == 0)
                    return 0;
                return Math.Round((double)GetReviewsCount() / (double) GetTutorsCount(),2);
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public int GetRequestsCount()
        {
            try
            {
                return _context.TutorRequests.Count();
            }
            catch(Exception e)
            {
                return 0;
            }
        }

        public List<SchoolClass> GetTopFiveRequestingClasses()
        {
            try
            {
                return _context.TutorRequests.GroupBy(tr => tr.SchoolClass).Select(grp =>
                new
                {
                    SchoolClass = _context.SchoolClasses.Where(s => s.Name == grp.Key).FirstOrDefault(),
                    count = grp.Count()
                }).OrderByDescending(a => a.count).Take(5).Select(a => a.SchoolClass).ToList();
            }
            catch(Exception e)
            {
                return new List<SchoolClass>();
            }
        }

        public Tutor GetMostRequestedTutor()
        {
            try
            {
                int id = _context.TutorRequests.Include("Class").GroupBy(r => r.Tutor_Id).Select(a =>
                new
                {
                    Tutor_Id = a.Key,
                    sum = a.Count()
                }).OrderByDescending(a => a.sum).FirstOrDefault().Tutor_Id;
                return _context.Tutors.Where(t => t.Id == id).FirstOrDefault();
            }
            catch(Exception e)
            {
                return new Tutor();
            }
        }

        public List<DataPoint> GetMonthsWithMostRequests()
        {
            List<DataPoint> points = new List<DataPoint>();
            try
            {
                for (int i = 0; i < 12; i++)
                {
                    var d = new DataPoint();
                    d.Label = GetString(i + 1);
                    d.Y = _context.TutorRequests.Count(r => r.Date.Month == i + 1);
                    points.Add(d);
                }
            }
            catch(Exception e)
            {

            }
            return points;
        }
        private string GetString(int month)
        {
            try
            {
                return new DateTime(2000, month, 1).ToString("MMMM", CultureInfo.CreateSpecificCulture("de"));
            }
            catch(Exception e)
            {
                return "-";
            }
        }

        public List<DataPoint> GetRequestPercentageOnTutorsWithImage()
        {
            List<DataPoint> list = new List<DataPoint>();
            try
            {
                double noimage = Math.Round((double)_context.TutorRequests.Include("Tutor").Count(tr => !String.IsNullOrEmpty(tr.Tutor.Image)) / (double)GetRequestsCount() * 100, 2);
                list.Add(new DataPoint() { Label = "Mit Bild", Y = 100 - noimage });
                list.Add(new DataPoint() { Label = "Ohne Bild", Y = noimage });
            }
            catch(Exception e)
            {

            }
            return list;
        }

        public string GetTeacherWithMostComments()
        {
            try
            {
                return _context.TeacherCommentStatistics.GroupBy(t => t.TeacherIdentityName).OrderByDescending(grp => grp.Count()).FirstOrDefault().Key;
            }
            catch(Exception e)
            {
                return "-";
            }
        }
        public List<DataPoint> GetAcceptedReviews()
        {
            List<DataPoint> list = new List<DataPoint>();
            try
            {
                int accepted = _context.AcceptStatistics.Count(a => a.ReviewAccepted != null && (bool)a.ReviewAccepted);
                int declined = _context.AcceptStatistics.Count(a => a.ReviewAccepted != null && (bool)a.ReviewAccepted == false);
                list.Add(new DataPoint() { Label = "Akzeptiert", Y = accepted });
                list.Add(new DataPoint() { Label = "Abgelehnt", Y = declined });
            }
            catch(Exception e)
            {

            }
            return list;
        }
        public List<DataPoint> GetAcceptedTutors()
        {
            List<DataPoint> list = new List<DataPoint>();
            try
            {
                int accepted = _context.AcceptStatistics.Count(a => a.TutorAccepted != null && (bool)a.TutorAccepted);
                int declined = _context.AcceptStatistics.Count(a => a.TutorAccepted != null && (bool)a.TutorAccepted == false);
                list.Add(new DataPoint() { Label = "Akzeptiert", Y = accepted });
                list.Add(new DataPoint() { Label = "Abgelehnt", Y = declined });
            }
            catch(Exception e)
            {

            }
            return list;
        }
    }
}
