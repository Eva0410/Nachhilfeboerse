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
        public void FillTutors(IUnitOfWork uow)
        {
            if (!String.IsNullOrEmpty(this.SortText))
            {
                switch (this.SortText)
                {
                    case "Name":
                        this.Tutors = uow.TutorRepository.Get(orderBy: ord => ord.OrderBy(t => t.LastName)).ToList();
                        break;
                    case "Geld":
                        this.Tutors = uow.TutorRepository.Get(orderBy: ord => ord.OrderBy(t => t.Price).ThenBy(t => t.LastName)).ToList();
                        break;
                    case "Schulstufe":
                        this.Tutors = uow.TutorRepository.Get(orderBy: ord => ord.OrderBy(t => t.Class.Name).ThenBy(t => t.LastName)).ToList();
                        break;
                }
            }

            else
                this.Tutors = uow.TutorRepository.Get(orderBy: ord => ord.OrderBy(t => t.LastName)).ToList();
        }

    }

}



