using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TutoringMarket.Core.Contracts;
using TutoringMarket.Core.Enities;

namespace TutoringMarket.WebIdentity.Models.ViewModels
{
    public class IndexModel
    {
        public List<Tutor> Tutors { get; set; }
        public String SortText { get; set; }
        public String SortTextBefore { get; set; }
        public SelectList Subjects { get; set; }
        public string SelectedSubject { get; set; }

        //public void SwitchSort(IUnitOfWork uow)
        //{
        //    switch (this.SortText)
        //    {
        //        case "Geld":
        //            this.Tutors = uow.TutorRepository.Get(orderBy: ord => ord.OrderBy(t => t.Price).ThenBy(t => t.LastName), includeProperties: "Class,Department, Tutor_Subjects, Tutor_Subjects.Subject").ToList();
        //            break;
        //        case "Schulstufe":
        //            this.Tutors = uow.TutorRepository.Get(orderBy: ord => ord.OrderBy(t => t.Class.Name).ThenBy(t => t.LastName), includeProperties: "Class,Department, Tutor_Subjects").ToList();
        //            break;
        //        case "Bewertung":
        //            this.Tutors = uow.GetTutorsByReviews();
        //            break;
        //        default:
        //            this.Tutors = uow.TutorRepository.Get(orderBy: ord => ord.OrderBy(t => t.LastName), includeProperties: "Class,Department, Tutor_Subjects").ToList();
        //            break;
        //    }
        //}
        public void FillTutors(IUnitOfWork uow)
        {
            //TODO: Sort client-seitig?
            //TODO: Subjects von tutor_subject mitladen, tutor_subject.subject, weniger laden
            //if (String.IsNullOrEmpty(this.SortTextBefore) || this.SortTextBefore != this.SortText)
            //{
            //    SwitchSort(uow);
            //    this.SortTextBefore = this.SortText;
            //}
            //else if (this.SortTextBefore == this.SortText)
            //{
            //    this.SortText = "";
            //    this.SortTextBefore = "";
            //    SwitchSort(uow);
            //}
            var subs = uow.SubjectRepository.Get(orderBy: ord => ord.OrderBy(s => s.Name)).Select(s => s.Name).ToList();
            subs.Insert(0,"Alle");
            this.Subjects = new SelectList(subs, "Alle");

            if (this.SelectedSubject == "Alle" || this.SelectedSubject == null)
            {
                this.Tutors = uow.TutorRepository.Get(orderBy: ord => ord.OrderBy(t => t.LastName),includeProperties:"Department, Class, Tutor_Subjects").ToList();
            }
            else
            {
                this.Tutors = uow.TutorSubjectRepository.Get(filter: ts => ts.Subject.Name == this.SelectedSubject, orderBy: ord => ord.OrderBy(ts => ts.Tutor.LastName), includeProperties:"Subject, Tutor").Select(ts => ts.Tutor).ToList();
            }
        }

    }

}





