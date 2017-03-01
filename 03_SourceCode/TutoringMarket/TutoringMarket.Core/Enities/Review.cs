using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    }
}
