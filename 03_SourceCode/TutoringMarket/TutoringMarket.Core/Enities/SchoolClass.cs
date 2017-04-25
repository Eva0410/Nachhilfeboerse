using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoringMarket.Core.Enities
{
    public class SchoolClass : EntityObject
    {
        [Required,Display(Name="Klasse")]
        public String Name { get; set; }
    }
}
