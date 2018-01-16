using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TutoringMarket.Core.Contracts;
using TutoringMarket.Core.Enities;

namespace TutoringMarket.WebIdentity.Models.ViewModels
{
    public class AdminMailForm
    {
        [Required]
        public string Nachricht { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Tutor Tutor { get; set; }
        public int ID { get; set; }

        public async Task Init(IUnitOfWork uow, int id)
        {
            InitTutor(uow, id);
            //set name, class
            this.FirstName = Tutor.FirstName;
            this.LastName = Tutor.LastName;
            this.ID = id;

            InitTutor(uow, id);
        }

        public void InitTutor(IUnitOfWork uow, int id)
        {
            this.Tutor = uow.TutorRepository.GetById(id);
        }
    }
}
