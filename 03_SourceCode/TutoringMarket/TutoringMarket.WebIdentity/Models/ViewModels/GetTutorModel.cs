using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TutoringMarket.Core.Contracts;
using TutoringMarket.Core.Enities;

namespace TutoringMarket.WebIdentity.Models.ViewModels
{
    public class GetTutorModel
    {
        public SelectList Classes { get; set; }
        public Tutor Tutor { get; set; }
        public SelectList Departments { get; set; }
        public List<int> SelectedSubjects { get; set; }
        public SelectList AvailableSubjects { get; set; }
        public SelectList Gender { get; set; }
        //not used
        public IFormFile Image { get; set; }
        public void Init(IUnitOfWork uow)
        {
            this.Tutor = new Tutor();
            this.SelectedSubjects = new List<int>();

            FillList(uow);
        }
        public void FillList(IUnitOfWork uow)
        {
            var depts = uow.DepartmentRepository.Get(orderBy: ord => ord.OrderBy(d => d.Name)).ToList();
            this.Departments = new SelectList(depts, "Id", "Name");

            var classes = uow.ClassRepository.Get(orderBy: ord => ord.OrderBy(c => c.Name)).ToList();
            this.Classes = new SelectList(classes, "Id", "Name");

            var gender = new List<string> { "Männlich", "Weiblich" };
            this.Gender = new SelectList(gender);

            var subjects = uow.SubjectRepository.Get(orderBy: ord => ord.OrderBy(s => s.Name)).ToList();
            this.AvailableSubjects = new SelectList(subjects, "Id", "Name");
        }
    }
}
