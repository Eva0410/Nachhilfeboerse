using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TutoringMarket.WebIdentity.Models
{
    public class EmailFormModel
    {
        [Required, Display(Name = "Dein Name")]
        public string FromName { get; set; }
        [Required, Display(Name = "Deine E-Mail"), EmailAddress]
        public string FromEmail { get; set; }
        [Required]
        public string Nachricht { get; set; }
    }
}
