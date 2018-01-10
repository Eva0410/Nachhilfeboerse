using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TutoringMarket.Core.Contracts;
using TutoringMarket.Core.Enities;

namespace TutoringMarket.WebIdentity.Models.ViewModels
{
    public class TutorModel
    {
        public Tutor Tutor { get; set; }
        public string filter { get; set; }
        public string sort { get; set; }
        //not used yet
        //public Review[] Reviews { get; set; }
        //public double Average { get; set; }
        public void Init(IUnitOfWork uow, int id)
        {
            this.Tutor = uow.TutorRepository.Get(filter: t => t.Id == id, includeProperties:"Department, Class, Subjects").FirstOrDefault();
            //this.Reviews = uow.ReviewRepository.Get(filter: r => r.Tutor_Id == id && r.Approved == true);
            //if (this.Reviews.Length != 0)
            //    this.Average = this.Reviews.Average(r => r.Books);
            //else
            //    this.Average = 0;
        }
    }
}
