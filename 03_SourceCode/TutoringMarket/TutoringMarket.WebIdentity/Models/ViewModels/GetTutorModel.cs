using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TutoringMarket.Core.Contracts;
using TutoringMarket.Core.Enities;
using System.Web;

namespace TutoringMarket.WebIdentity.Models.ViewModels
{
    public class GetTutorModel
    {
        public HttpPostedFileBase ImageFile { get; set; }
        public Tutor Tutor { get; set; }
        public List<int> SelectedSubjects { get; set; }
        public SelectList AvailableSubjects { get; set; }
        public SelectList Gender { get; set; }
        //not used
        public String Image { get; set; }
        public async Task Init(IUnitOfWork uow, UserManager<ApplicationUser> um, string name)
        {
            this.Tutor = new Tutor();
            this.Tutor.Birthday = new DateTime(DateTime.Now.Year - 18, 1, 1); //default date
            this.SelectedSubjects = new List<int>();

            await FillList(uow, um, name);
        }
        //TODO beim Fächer auswählen, schönere Multi Select list ohne Crtl Taste
        public async Task FillList(IUnitOfWork uow, UserManager<ApplicationUser> um, string name)
        {
            ApplicationUser CurrentUser = await um.FindByNameAsync(name);
            //set name, class, department
            this.Tutor.FirstName = CurrentUser.FirstName;
            this.Tutor.LastName = CurrentUser.LastName;
            //Class and department should exist (inserted in accountcontroller when user logs in)
            var existingClass = uow.ClassRepository.Get(c => c.Name == CurrentUser.SchoolClass).FirstOrDefault();
            if (existingClass != null)
            {
                this.Tutor.Class = existingClass;
                this.Tutor.Class_Id = existingClass.Id;
            }
            else
            {
                SchoolClass newClass = new SchoolClass() { Name = CurrentUser.SchoolClass };
                uow.ClassRepository.Insert(newClass);
                uow.Save();
                this.Tutor.Class = newClass;
                this.Tutor.Class_Id = uow.ClassRepository.Get(c => c.Name == newClass.Name).FirstOrDefault().Id;
            }

            var existingDepartment = uow.DepartmentRepository.Get(d => d.Name == CurrentUser.Department).FirstOrDefault();
            if (existingDepartment != null)
            {
                this.Tutor.Department = existingDepartment;
                this.Tutor.Department_Id = existingDepartment.Id;
            }
            else
            {
                Department newDeparment = new Department() { Name = CurrentUser.Department };
                uow.DepartmentRepository.Insert(newDeparment);
                uow.Save();
                this.Tutor.Department = newDeparment;
                this.Tutor.Department_Id = uow.DepartmentRepository.Get(d => d.Name == newDeparment.Name).FirstOrDefault().Id;
            }

            var gender = new List<string> { "Männlich", "Weiblich" };
            this.Gender = new SelectList(gender);

            var subjects = uow.SubjectRepository.Get(orderBy: ord => ord.OrderBy(s => s.Name)).ToList();
            this.AvailableSubjects = new SelectList(subjects, "Id", "Name");
        }
    }
}
