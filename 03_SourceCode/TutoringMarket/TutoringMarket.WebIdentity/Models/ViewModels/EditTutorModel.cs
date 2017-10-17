using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TutoringMarket.Core.Contracts;
using TutoringMarket.Core.Enities;

namespace TutoringMarket.WebIdentity.Models.ViewModels
{
    public class EditTutorModel
    {
        public SelectList Classes { get; set; }
        public Tutor Tutor { get; set; }
        public SelectList Departments { get; set; }
        public List<int> SelectedSubjects { get; set; }
        public SelectList AvailableSubjects { get; set; }
        public SelectList Gender { get; set; }
        //not used
        public IFormFile Image { get; set; }

        public async Task FillList(IUnitOfWork uow, UserManager<ApplicationUser> um, ClaimsPrincipal user)
        {
            this.Tutor = uow.TutorRepository.Get(t => t.IdentityName == user.Identity.Name, includeProperties:"Subjects").FirstOrDefault();
            ApplicationUser CurrentUser = await um.FindByNameAsync(user.Identity.Name);
            this.Tutor.FirstName = CurrentUser.FirstName;
            this.Tutor.LastName = CurrentUser.LastName;
            var existingClass = uow.ClassRepository.Get(c => c.Name == CurrentUser.SchoolClass).FirstOrDefault();
            if (existingClass != null)
            {
                this.Tutor.Class = existingClass;
                this.Tutor.Class_Id = existingClass.Id;
            }

            var existingDepartment = uow.DepartmentRepository.Get(d => d.Name == CurrentUser.Department).FirstOrDefault();
            if (existingDepartment != null)
            {
                this.Tutor.Department = existingDepartment;
                this.Tutor.Department_Id = existingDepartment.Id;
            }

            var gender = new List<string> { "Männlich", "Weiblich" };
            this.Gender = new SelectList(gender);

            var subjects = uow.SubjectRepository.Get(orderBy: ord => ord.OrderBy(s => s.Name)).ToList();
            this.AvailableSubjects = new SelectList(subjects, "Id", "Name");

            this.SelectedSubjects = this.Tutor.Subjects.Select(s => s.Id).ToList();
        }
    }
}
