using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoringMarket.Core.Enities
{
    public class Subject : EntityObject
    {
        [Required,Display(Name="Fach")]
        public String Name { get; set; }
        public List<Tutor_Subject> Tutor_Subjects { get; set; }
    }
}
