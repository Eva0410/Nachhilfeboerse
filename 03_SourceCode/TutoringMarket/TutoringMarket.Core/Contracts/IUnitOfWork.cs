using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoringMarket.Core.Enities;
using TutoringMarket.Core.Statistics;

namespace TutoringMarket.Core.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        
        IGenericRepository<Tutor> TutorRepository { get; }
        IGenericRepository<SchoolClass> ClassRepository { get; }
        IGenericRepository<Department> DepartmentRepository { get; }
        IGenericRepository<Review> ReviewRepository { get; }
        IGenericRepository<Subject> SubjectRepository { get; }
        IGenericRepository<TeacherComment> TeacherCommentRepository { get; }
        IGenericRepository<TutorRequest> TutorRequestRepository { get; }

        void Save();

        void DeleteDatabase();
        List<Tutor> GetTutorsByReviews();
        List<DataPoint> GetTutorsPerGender();
        List<DataPoint> GetTutorsPerClass();
        string GetDepartmentWithMostTutors();
        int GetTutorsCount();
        int GetTutorsWithImagePercentage();

        int GetReviewsCount();
        double GetAverageReview();
        double GetAverageCountReviewsPerTutor();

        int GetRequestsCount();
        List<SchoolClass> GetTopFiveRequestingClasses();
        Tutor GetMostRequestedTutor();
        List<DataPoint> GetMonthsWithMostRequests();
        List<DataPoint> GetRequestPercentageOnTutorsWithImage();
    }
}
