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
        public void Init(IUnitOfWork uow)
        {
            this.OutstandingTutors = uow.TutorRepository.Get(filter: t => t.Accepted == false, includeProperties: "Subjects, Class, Department, Comments").ToList();
            this.Tutor_Id = 0;
            this.Comment = "";
        }
    }
}
