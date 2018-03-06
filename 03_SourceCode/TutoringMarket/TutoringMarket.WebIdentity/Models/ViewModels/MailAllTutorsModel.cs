using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TutoringMarket.WebIdentity.Models.ViewModels
{
    public class MailAllTutorsModel
    {
        [Required]
        public String Message { get; set; }
        [Required]
        public String Subject { get; set; }
    }
}
