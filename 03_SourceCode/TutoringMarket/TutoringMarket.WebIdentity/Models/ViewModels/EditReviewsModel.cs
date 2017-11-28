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
        //TOOD implement
        public List<Review> Reviews { get; set; }
        public int ReviewId { get; set; }
        public void Init(IUnitOfWork uow)
        {
            this.Reviews = uow.ReviewRepository.Get(filter: r => r.Approved == false, includeProperties:"Tutor").ToList();
        }
    }
}
