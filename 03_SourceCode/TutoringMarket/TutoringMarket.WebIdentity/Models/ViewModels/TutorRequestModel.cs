
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
    public class TutorRequestModel
    {
        [Required]
        public string Message { get; set; }
        public Tutor Tutor { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string  SchoolClass { get; set; }
        [Required]
        public string Comments { get; set; }
        [Required]
        public string Ending { get; set; }
        public async Task Init(IUnitOfWork uow, int id, UserManager<ApplicationUser> um, string name)
        {
            ApplicationUser CurrentUser = await um.FindByNameAsync(name);
            //set name, class
            this.FirstName = CurrentUser.FirstName;
            this.LastName = CurrentUser.LastName;
            this.SchoolClass = CurrentUser.SchoolClass;

            InitTutor(uow, id);
            if(this.Tutor != null)
                this.Message = String.Format("Hallo {0}! {1}{1}Ich habe dein Profil online auf der Nachhilfebörse gesehen und möchte deine Nachhilfe gerne in Anspruch nehmen. Ich heiße {2} {3} und gehe in die {4}. Wenn du Interesse hast, melde dich bitte bei mir. {1} {1}", this.Tutor.FirstName, Environment.NewLine, FirstName, LastName, SchoolClass);
            this.Comments = String.Format("Meine Daten sind: {0}E-Mail: <E-Mail Adresse> {0}Telefon: <Telefonnummer> {0}Fächer, in denen ich Nachhilfe brauche: <Fächer> {0}{0} <Hier kannst du eventuelle Anmerkungen angeben>", Environment.NewLine);
            this.Ending = String.Format("{0}{0}Ich freue mich schon sehr auf deine Antwort! {0}{0}Liebe Grüße {0}{1}", Environment.NewLine, FirstName);
        }
        public void InitTutor(IUnitOfWork uow, int id)
        {
            this.Tutor = uow.TutorRepository.GetById(id);
        }
    }
}
