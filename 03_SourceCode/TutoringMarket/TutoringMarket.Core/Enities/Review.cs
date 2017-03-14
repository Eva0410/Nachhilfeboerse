using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoringMarket.Core.Enities
{
    public class Review : EntityObject
    {
        [Required, Display(Name ="Bücher")]
        public int Books { get; set; }

        [Required, Display(Name="Kommentar")]
        public String Comment { get; set; }
        [Required, Display(Name ="Tutor"), ForeignKey("Tutor_Id")]
        public Tutor Tutor { get; set; }
        public int Tutor_Id { get; set; }
        [Required]
        public bool Approved { get; set; }
    }
}
