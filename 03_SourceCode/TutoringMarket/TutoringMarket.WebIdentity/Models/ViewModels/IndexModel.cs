using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            
            //TODO Model ändern
            if (this.SelectedSubject == "Alle" || this.SelectedSubject == null)
            {
                this.Tutors = uow.TutorRepository.Get(orderBy: ord => ord.OrderBy(t => t.LastName), includeProperties:"Class, Department, Subjects").ToList();
            }
            else
            {
                //TODO oben im Login_Partial den Namen angeben und nicht in130021
                //TODO wenn model geändert ist, kompliziertn teil löschen!!!!
                this.Tutors = uow.TutorRepository.Get(filter: t => t.Subjects.Where(s => s.Name == this.SelectedSubject).ToList().Count > 0, orderBy: ord => ord.OrderBy(t => t.LastName), includeProperties: "Class, Department, Subjects").ToList();
                //var tutors = uow.TutorSubjectRepository.Get(filter: ts => ts.Subject.Name == this.SelectedSubject, orderBy: ord => ord.OrderBy(ts => ts.Tutor.LastName), includeProperties:"Subject, Tutor").Select(ts => ts.Tutor).ToList();
                //List<Tutor> includedList = new List<Tutor>();
                //foreach (var item in tutors)
                //{
                //    Tutor tut = uow.TutorRepository.Get(filter: t => t.Id == item.Id, includeProperties: "Department, Class, Tutor_Subjects").FirstOrDefault();
                //    var subjects = tut.Tutor_Subjects;
                //    item.Subjects = new List<Subject>();
                //    foreach (var s in subjects)
                //    {
                //        tut.Subjects.Add(uow.SubjectRepository.GetById(s.Subject_Id));
                //    }
                //    includedList.Add(tut);
                //}
                //this.Tutors = includedList;
            }
        }

    }

}





