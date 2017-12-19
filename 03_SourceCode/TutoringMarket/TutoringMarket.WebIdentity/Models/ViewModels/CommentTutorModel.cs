using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TutoringMarket.Core.Contracts;
using TutoringMarket.Core.Enities;

namespace TutoringMarket.WebIdentity.Models.ViewModels
{
    public class CommentTutorModel
    {
        public List<Tutor> OutstandingTutors { get; set; }
        //Id of the tutor which has been commented on
        public int Tutor_Id { get; set; }
        public string Comment { get; set; }
        public SelectList Subjects { get; set; }
        public string SelectedSubject { get; set; }
        public void Init(IUnitOfWork uow)
        {
            this.Tutor_Id = 0;
            this.Comment = "";

            var subs = uow.SubjectRepository.Get(orderBy: ord => ord.OrderBy(s => s.Name)).Select(s => s.Name).ToList();
            subs.Insert(0, "Alle");
            this.Subjects = new SelectList(subs, "Alle");

            if (this.SelectedSubject == "Alle" || this.SelectedSubject == null)
            {
                this.OutstandingTutors = uow.TutorRepository.Get(filter: t => t.Accepted == false, orderBy: ord => ord.OrderBy(t => t.LastName), includeProperties: "Class, Department, Subjects, Comments").ToList();
            }
            else
            {
               
                this.OutstandingTutors = uow.TutorRepository.Get(filter: t => t.Subjects.Where(s => s.Name == this.SelectedSubject).ToList().Count > 0 && t.Accepted == false, orderBy: ord => ord.OrderBy(t => t.LastName), includeProperties: "Class, Department, Subjects, Comments").ToList();
            }
        }
    }
}
