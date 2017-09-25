﻿using System;
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
        public Review[] Reviews { get; set; }
        public double Average { get; set; }
        public List<Subject> Subjects { get; set; }
        public void Init(IUnitOfWork uow, int id)
        {
            this.Tutor = uow.TutorRepository.Get(filter: t => t.Id == id, includeProperties:"Department, Class, Tutor_Subjects").FirstOrDefault();
            this.Reviews = uow.ReviewRepository.Get(filter: r => r.Tutor_Id == id && r.Approved == true);
            if (this.Reviews.Length != 0)
                this.Average = this.Reviews.Average(r => r.Books);
            else
                this.Average = 0;
            this.Subjects = uow.TutorSubjectRepository.Get(ts => ts.Tutor_Id == this.Tutor.Id, includeProperties:"Subject").Select(ts => ts.Subject).ToList();
        }
    }
}