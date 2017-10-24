using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoringMarket.Core.Enities
{
    public class Tutor_Subject : EntityObject
    {
        [Display(Name = "Nachhilfelehrer"), ForeignKey("Tutor_Id")]
        public Tutor Tutor { get; set; }
        public int Tutor_Id { get; set; }

        [Display(Name = "Fach"), ForeignKey("Subject_Id")]
        public Subject Subject { get; set; }
        public int Subject_Id { get; set; }
    }
}