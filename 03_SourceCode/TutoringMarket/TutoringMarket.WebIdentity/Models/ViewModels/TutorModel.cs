using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using TutoringMarket.Core.Contracts;
using TutoringMarket.Core.Enities;

namespace TutoringMarket.WebIdentity.Models.ViewModels
{
    public class TutorModel
    {
        [ForeignKey("Tutor_Id")]
        public Tutor Tutor { get; set; }
        public string filter { get; set; }
        public string sort { get; set; }
        public Review[] Reviews { get; set; }
        public double Average { get; set; }
        public Review NewReview { get; set; }
        public int Tutor_Id { get; set; }
        public void Init(IUnitOfWork uow)
        {
            this.Tutor = uow.TutorRepository.Get(filter: t => t.Id == this.Tutor_Id, includeProperties:"Department, Class, Subjects").FirstOrDefault();
            this.Reviews = uow.ReviewRepository.Get(filter: r => r.Tutor_Id == this.Tutor_Id && r.Approved == true);
            if (this.Reviews.Length != 0)
                this.Average = Math.Round(this.Reviews.Average(r => r.Books),1);
            else
                this.Average = 0;
            NewReview = new Review();
            NewReview.Books = 1;
        }
    }
}
