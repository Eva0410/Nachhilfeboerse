using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoringMarket.Core.Enities
{
    public class Department : EntityObject
    {
        [Required,Display(Name="Abteilung")]
        public String Name { get; set; }
    }
}
