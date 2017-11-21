using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoringMarket.Core.Enities
{
    public class TeacherComment : EntityObject
    {
        [Required(ErrorMessage ="Bitte geben Sie einen Kommentar ein!")]
        public string Comment { get; set; }
        //Bsp "p.bauer"
        [Required]
        public string TeacherIdentityName { get; set; }
        [ForeignKey("Tutor_Id")]
        public Tutor Tutor { get; set; }
        public int Tutor_Id { get; set; }
    }
}
