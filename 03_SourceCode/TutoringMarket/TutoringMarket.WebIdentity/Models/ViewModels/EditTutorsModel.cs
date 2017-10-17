﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TutoringMarket.Core.Contracts;
using TutoringMarket.Core.Enities;

namespace TutoringMarket.WebIdentity.Models.ViewModels
{
    public class EditTutorsModel
    {
        public List<Tutor> Tutors { get; set; }
        public List<Subject> Subjects { get; set; }
        public List<Tutor> OutstandingTutors { get; set; }

        public void Init(IUnitOfWork uow)
        {
            this.OutstandingTutors = uow.TutorRepository.Get(filter: t => t.Accepted == false, includeProperties:"Subjects, Class, Department").ToList();
            //this.Tutors = uow.TutorRepository.Get(includeProperties: "Tutor_Subjects").ToList();
        }
    }
}
