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
        public string[] Comments { get; set; }
        public List<Tutor> OutstandingTutors { get; set; }
        public List<TeacherComment> NewComments { get; set; }

        public void Init(IUnitOfWork uow)
        {
            //this.OutstandingTutors = uow.TutorRepository.Get(filter: t => t.Accepted == false, includeProperties:"Subjects, Class, Department, Comments").ToList();
            //this.NewComments = new List<TeacherComment>();
            //foreach (var item in OutstandingTutors)
            //{
            //    TeacherComment comment = new TeacherComment();
            //    comment.Tutor = item;
            //    comment.Tutor_Id = item.Id;
            //    NewComments.Add(comment);
            //}
        }
    }
}
