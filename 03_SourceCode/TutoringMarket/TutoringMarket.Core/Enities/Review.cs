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
        [Required, Display(Name ="Bücher"),Range(1,5, ErrorMessage = "Der Wert muss zwischen 1 und 5 liegen!")]
        public int Books { get; set; }

        [Required(ErrorMessage ="Der Kommentar darf nicht leer sein!"), Display(Name="Kommentar")]
        public String Comment { get; set; }
        [Display(Name ="Tutor"), ForeignKey("Tutor_Id")]
        public Tutor Tutor { get; set; }
        public int Tutor_Id { get; set; }
        [Required]
        public bool Approved { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public string Author { get; set; }
    }
}
