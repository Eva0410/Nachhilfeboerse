using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TutoringMarket.Core.Contracts;
using TutoringMarket.Core.Enities;

namespace TutoringMarket.WebIdentity.Models.ViewModels
{
    public class EditTutorsModel
    {
        //Accepted tutors
        public List<Tutor> Tutors { get; set; }
        public List<Tutor> OutstandingTutors { get; set; }

        public void Init(IUnitOfWork uow)
        {
            this.OutstandingTutors = uow.TutorRepository.Get(filter: t => t.Accepted == false, includeProperties:"Subjects, Class, Department, Comments").ToList();
            this.Tutors = uow.TutorRepository.Get(filter: t => t.Accepted == true, orderBy: ord => ord.OrderBy(t => t.LastName), includeProperties: "Subjects, Class, Department").ToList();
        }
    }
}
