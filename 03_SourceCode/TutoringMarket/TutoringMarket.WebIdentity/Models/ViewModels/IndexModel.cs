using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TutoringMarket.Core.Contracts;
using TutoringMarket.Core.Enities;

namespace TutoringMarket.WebIdentity.Models.ViewModels
{
    public class IndexModel
    {
        public List<Tutor> Tutors { get; set; }
        public SelectList Subjects { get; set; }
        public string SelectedSubject { get; set; }
        public SelectList SortList { get; set; }
        public string SelectedSortProperty { get; set; }

        public void FillTutors(IUnitOfWork uow)
        {
            //defaul sort property
            this.SelectedSortProperty = SelectedSortProperty == null ? "Name - aufsteigend" : this.SelectedSortProperty;
            //fill sort and filter selectlist
            var sortprops = new List<String>() { "Name - aufsteigend", "Name - absteigend", "Schulstufe - aufsteigend", "Schulstufe - absteigend", "Preis - aufsteigend", "Preis - absteigend" };
            this.SortList = new SelectList(sortprops, this.SelectedSortProperty);

            var subs = uow.SubjectRepository.Get(orderBy: ord => ord.OrderBy(s => s.Name)).Select(s => s.Name).ToList();
            subs.Insert(0, "Alle");
            this.Subjects = new SelectList(subs, "Alle");

            //save property info for dynamic use
            PropertyInfo prop = null;
            switch (this.SelectedSortProperty.Split('-')[0].Trim())
            {
                case "Preis":
                    prop = typeof(Tutor).GetProperty("Price");
                    break;
                case "Name":
                    prop = typeof(Tutor).GetProperty("LastName");
                    break;
                case "Schulstufe":
                    prop = typeof(Tutor).GetProperty("Class");
                    break;
            }

            //save method (orderby or orderbydescending?) unfortunately an extra if has to be made for sorting by schoolclass (tutor.class.name not tutor.price)
            Func<IQueryable<Tutor>, IOrderedQueryable<Tutor>> order;
            if (this.SelectedSortProperty == null || this.SelectedSortProperty.Contains("aufsteigend"))
            {
                if (prop.Name == "Class")
                    order = (t) => t.OrderBy(f =>  ((SchoolClass)prop.GetValue(f, null)).Name);
                else 
                    order = (t) => t.OrderBy(f => prop.GetValue(f, null));
            }
            else
            {
                if (prop.Name == "Class")
                    order = (t) => t.OrderByDescending(f => ((SchoolClass)prop.GetValue(f, null)).Name);
                else
                    order = (t) => t.OrderByDescending(f => prop.GetValue(f, null));
            }

            //get filtered list from database
            if (this.SelectedSubject == "Alle" || this.SelectedSubject == null)
            {
                this.Tutors = uow.TutorRepository.Get(filter: t => t.Accepted == true, includeProperties: "Class, Department, Subjects").ToList();
            }
            else
            {
                //TODO oben im Login_Partial den Namen angeben und nicht in130021
                this.Tutors = uow.TutorRepository.Get(filter: t => t.Subjects.Where(s => s.Name == this.SelectedSubject).ToList().Count > 0 && t.Accepted == true, includeProperties: "Class, Department, Subjects").ToList();
            }

            //invoke order command after collecting the data from the database
            this.Tutors = order.Invoke(this.Tutors.AsQueryable()).ToList();
        }

    }

}





