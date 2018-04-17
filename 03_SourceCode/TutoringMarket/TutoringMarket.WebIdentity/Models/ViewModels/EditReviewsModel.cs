using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TutoringMarket.Core.Contracts;
using TutoringMarket.Core.Enities;

namespace TutoringMarket.WebIdentity.Models.ViewModels
{
    public class EditReviewsModel
    {
        public List<Review> Reviews { get; set; }
        public List<Review> AllReviews { get; set; }
        public void Init(IUnitOfWork uow)
        {
            this.Reviews = uow.ReviewRepository.Get(filter: r => r.Approved == false, includeProperties:"Tutor").ToList();
            this.AllReviews = uow.ReviewRepository.Get(filter: r => r.Approved, includeProperties: "Tutor").ToList();
        }
    }
}
