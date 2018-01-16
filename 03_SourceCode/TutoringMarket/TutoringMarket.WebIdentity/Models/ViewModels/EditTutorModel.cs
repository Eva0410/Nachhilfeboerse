using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TutoringMarket.Core.Contracts;
using TutoringMarket.Core.Enities;

namespace TutoringMarket.WebIdentity.Models.ViewModels
{
    public class EditTutorModel
    {
        public IFormFile Image { get; set; }
        [FileExtensions(Extensions = "jpg", ErrorMessage = "Bild darf nur im jpg-Format hochgeladen werden!")]
        public string ImageFileName
        {
            get
            {
                if (this.Image != null)
                    return this.Image.FileName;
                else
                    return null;
            }
        }
        public Tutor Tutor { get; set; }
        public List<int> SelectedSubjects { get; set; }
        public SelectList AvailableSubjects { get; set; }
        public SelectList Gender { get; set; }

        public async Task FillList(IUnitOfWork uow, UserManager<ApplicationUser> um, ClaimsPrincipal user)
        {
            ApplicationUser CurrentUser = await um.FindByNameAsync(user.Identity.Name);
            if (CurrentUser != null)
            {
                this.Tutor = uow.TutorRepository.Get(t => t.IdentityName == user.Identity.Name && t.OldTutorId != 0, includeProperties: "Subjects").FirstOrDefault();
                if (this.Tutor == null)
                    this.Tutor = uow.TutorRepository.Get(t => t.IdentityName == user.Identity.Name, includeProperties: "Subjects").FirstOrDefault();
                //Fill name, class, department
                this.Tutor.FirstName = CurrentUser.FirstName;
                this.Tutor.LastName = CurrentUser.LastName;
                //class and department should exist (are inserted in the accountcontroller when user logs in)
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
}
