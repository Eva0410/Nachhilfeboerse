using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoringMarket.Core.Contracts;
using TutoringMarket.Core.Enities;

namespace TutoringMarket.Core
{
    public class TutoringController
    {
        const string FILENAME = "TestTutors.csv";
        IUnitOfWork _unitOfWork;
        public TutoringController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public void FillDatabaseFromCsv()
        {

            string[][] csvTutors = FILENAME.ReadStringMatrixFromCsv(true);
            List<SchoolClass> classes = GetClasses("TestClasses.csv".ReadStringMatrixFromCsv(true));
            List<Subject> subjects = GetSubjects("TestSubjects.csv".ReadStringMatrixFromCsv(true));
            List<Department> departments = GetDepartments("TestDepartments.csv".ReadStringMatrixFromCsv(true));

            List<Tutor> tutors = csvTutors.Select(l =>
            new Tutor()
            {
                FirstName = l[0],
                LastName = l[1],
                EMail = l[2],
                PhoneNumber = l[3],
                Birthday = Convert.ToDateTime(l[4]),
                Time = l[5],
                Price = Convert.ToInt32(l[6]),
                Department = departments.Where(d => d.Name == l[7]).FirstOrDefault(),
                Class = classes.Where(c => c.Name == l[8]).FirstOrDefault(),
                Gender = l[9],
                IdentityName = l[10],
                Accepted = Boolean.Parse(l[11]),
                Subjects = l[12].Split(',').Select(s =>
                new
                {
                    sub = subjects.FirstOrDefault(su => su.Name == s)
                }).Select(a => a.sub).ToList()
            }
            ).ToList();

            List<Review> reviews = GetReviews(tutors);

            //throw new Exception("Bist du dir sicher, dass du die Datenbank zurücksetzen willst??"); //auskommentieren wenn schon

            _unitOfWork.DeleteDatabase();
            _unitOfWork.ClassRepository.InsertMany(classes);
            _unitOfWork.SubjectRepository.InsertMany(subjects);
            _unitOfWork.DepartmentRepository.InsertMany(departments);
            _unitOfWork.TutorRepository.InsertMany(tutors);
            _unitOfWork.ReviewRepository.InsertMany(reviews);
            _unitOfWork.Save();

        }
        public List<Review> GetReviews(List<Tutor> tutors)
        {
            return "TestReviews.csv".ReadStringMatrixFromCsv(true).Select(l =>
               new Review()
               {
                   Tutor = tutors.SingleOrDefault(t => t.LastName == (l[0])),
                   Books = Convert.ToInt32(l[1]),
                   Comment = l[2],
                   Approved = Convert.ToBoolean(l[3])
               }).ToList();
        }
        public List<Subject> GetSubjects(string[][] list)
        {
            return list.Select(l =>
               new Subject()
               {
                   Name = l[0]
               }).ToList();
        }
        public List<Department> GetDepartments(string[][] list)
        {
            return list.Select(l =>
            new Department()
            {
                Name = l[0]
            }).ToList();
        }
        public List<SchoolClass> GetClasses(string[][] list)
        {
            return list.Select(l =>
            new SchoolClass()
            {
                Name = l[0]
            }).ToList();
        }
    }
}
